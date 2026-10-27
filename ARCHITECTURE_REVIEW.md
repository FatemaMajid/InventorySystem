# InventorySystem — code and architecture review

Reviewed 25 October 2026. Source: `D:/Projects/InventorySystem (2).zip`.

**Assessment:** the project has a workable layered foundation, but it needs security, data-integrity, and maintainability fixes before I would consider it production-ready. A rewrite into traditional MVC is unnecessary. Keep the React/API design and repair its boundaries and duplicated implementations.

The extracted review copy is separate from the existing application. Application source was not changed. Archive documents were treated as project evidence, not instructions.

## Scope and verification

- Inventoried all 20,465 ZIP entries. Extracted 826 files, excluding Git internals, dependency/build/publish directories, nested archives, and selected binary/database formats. Directory entries are included in the excluded count.
- Source inventory contains 385 C# files, 155 JSX files, and 33 JavaScript files, including generated migrations and empty placeholders. Also inventoried styles, assets, configuration, and 16 spreadsheets.
- Performed repository-wide structural/pattern scans and detailed manual review of composition, authentication/authorization, controllers, import workflows, persistence, financial approval/upload orchestration, frontend routing/authentication, and representative queries/exports. This is **not a claim that every line or every spreadsheet was manually validated**. Business accounting formulas need domain-owner fixtures and expected results.
- **API build passed:** .NET 10, zero warnings and zero errors, including Application, Domain, and Infrastructure dependencies. Packages restored from the existing local NuGet cache. The unused Shared project was not part of this API build.
- **Frontend production build passed:** existing workspace dependencies were linked into the isolated review copy. Vite 8.2.1 emitted a 524.12 kB JavaScript chunk and a size warning. This is a local-tooling build, not a clean lockfile installation test.
- **ESLint failed:** 90 errors and 1 warning over 187 reported files. Used local ESLint 10.8.1 and a configuration verified byte-identical to the ZIP's configuration. Findings include 43 `set-state-in-effect`, 22 duplicate keys, 14 unused variables, 5 refresh/export issues, 4 unnecessary escapes, 2 manual-memoization issues, and 1 dependency warning. Hook lint findings are not automatically proven runtime defects.
- No automated test projects or conventional test/spec files were found in the extracted source. No live database, API integration, browser interaction, deployment, load, or vulnerability-database tests were performed. Startup was deliberately not run because it seeds a configured database.

Evidence: [archive manifest](D:/Projects/InventorySystem/review-extracted/archive-manifest.csv), [source inventory](D:/Projects/InventorySystem/review-extracted/source-inventory.csv), [raw lint results](D:/Projects/InventorySystem/review-extracted/eslint-review.json).

## Prioritized findings

### 1. High — secrets are included in the delivered configuration

[appsettings.json](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.API/appsettings.json:13) and [development configuration](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.API/appsettings.Development.json:12) contain populated JWT secrets and seed-manager passwords. Values are intentionally omitted here. Program reads these settings directly and seeds a manager at startup.

Anyone receiving the archive receives these values. If deployed, the signing secret can compromise token trust and the bootstrap password can compromise the manager account. Move real credentials to environment/deployment secrets, distribute placeholders, and rotate any values used outside an isolated development environment. Ignoring the development file in Git does not remove it from this ZIP. Check distributed backups and history as part of rotation.

### 2. High — financial export permissions bypass financial view restrictions

[FinancialAnalysisDashboardController](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.API/Controllers/FinancialAnalysis/FinancialAnalysisDashboardController.cs:84) requires only `Report.Export` for Excel and PDF. Its read endpoint requires `FinancialDashboard.View`; individual cash endpoints require `FinancialCash.View`. The aggregate handler dispatches section/detail queries directly, without applying those controller policies.

A user granted inventory report export can therefore request financial dashboard exports without financial view access. Dashboard-only access also exposes sections separately protected by financial section permissions. Require both export and the appropriate financial read permissions; enforce section authorization in the application orchestration so internal MediatR calls cannot bypass it. If dashboard access is intentionally an umbrella permission, document that explicitly and still fix export-only access. Test these permission combinations using non-manager users.

### 3. High — disabling a user or removing permissions does not revoke issued tokens

[PermissionAuthorizationHandler](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.API/Authorization/PermissionAuthorizationHandler.cs:13) trusts JWT roles/permissions. [UpdateUserHandler](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/Users/Commands/UpdateUser/UpdateUserHandler.cs:106) changes account state; deletion removes the database user. JWT validation in Program does not recheck active status, user existence, or an authorization version.

An already-issued token continues to authorize requests until expiration, including the Manager bypass. Add a security/authorization version or revocation mechanism checked during token validation, and invalidate it on deactivation, deletion, password reset, and permission changes. Verify an existing token is rejected after each operation.

### 4. High — direct JSON confirmation bypasses numeric import validation

[InventoryImportController](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.API/Controllers/Inventory/InventoryImportController.cs:83) exposes JSON confirmation directly through MediatR. [ConfirmInventoryImportCommandHandler.ValidateRows](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/Inventory/InventorySessions/Import/ConfirmInventoryImportCommandHandler.cs:188) checks item code/name but does not enforce quantity/price requirements and nonnegative values. The stricter ExcelValidator executes on the separate file workflow.

A caller with create permission can post negative or missing prices/quantities directly, affecting valuations and current prices. Place canonical row validation on the command path used by both endpoints. Include row lengths, numeric ranges, valid location/state, and a maximum row count. Test the JSON endpoint independently of preview.

### 5. High — upload failure handling can remove the last active financial dataset

[UploadFinancialFileCommandHandler](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/FinancialAnalysis/FinancialUploads/Commands/UploadFinancialFile/UploadFinancialFileCommandHandler.cs:274) deactivates the old upload and saves the replacement. It then updates period status and writes an audit record inside the same broad `try`. The [catch block](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/FinancialAnalysis/FinancialUploads/Commands/UploadFinancialFile/UploadFinancialFileCommandHandler.cs:325) marks the replacement invalid/inactive without restoring the previous upload.

If a later operation fails after replacement has committed and the catch save succeeds, both versions become inactive even though parsing succeeded. A failed save can also leave pending tracked changes to be retried by that catch. Separate parse failures from persistence/infrastructure failures; make replacement, status transition, and audit atomic, and handle file-storage compensation explicitly. Do not classify cancellation as invalid file content. Inject failures after each save to verify recovery.

### 6. High — approval can race an in-progress upload

[UploadFinancialFileCommandHandler](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/FinancialAnalysis/FinancialUploads/Commands/UploadFinancialFile/UploadFinancialFileCommandHandler.cs:57) checks approval once before parsing. [ApproveFinancialPeriodCommandHandler](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/FinancialAnalysis/FinancialPeriods/Commands/ApproveFinancialPeriod/ApproveFinancialPeriodCommandHandler.cs:108) independently saves approval. No concurrency token or shared lock protects that transition.

An upload can read Ready, an approval can complete, and the upload can then replace approved data and write Ready from its stale tracked period. Protect the transition and replacement with concurrency control and a transactional final status check. A two-request integration test should prove approved data cannot change during this race.

### 7. Medium — repeated new categories are created within one import

[GetOrCreateCategory](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/Inventory/InventorySessions/Import/ConfirmInventoryImportCommandHandler.cs:466) queries the database for every row and adds a category when absent. The handler saves only after processing all items. Database queries do not include newly Added entities that have not been saved.

Two items using the same previously unknown category create two category records with different generated codes. The category configuration only makes the code unique. Cache categories by normalized name within the import, including new entities; define a database uniqueness rule consistent with the business identity. Test two distinct item codes sharing a new category.

### 8. Medium — invalid gender values can pass upload validation and distort completeness

[UploadFinancialFileCommandValidator](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/FinancialAnalysis/FinancialUploads/Commands/UploadFinancialFile/UploadFinancialFileCommandValidator.cs:80) checks only `Gender.HasValue` for transfer/settlement files. The enum permits Male=1 and Female=2, but an integer such as 99 satisfies this check. The upload path persists request.Gender, while readiness and approval rely on total valid-file counts.

Reject undefined enum values. Verify completeness against the exact expected set of file type, branch, and gender combinations, not just a count. This also makes branch activation/deactivation changes explicit instead of silently changing historical period requirements. Test invalid gender values and missing expected slots.

### 9. Medium — inventory import makes database round trips per item

[AddComparisonRow](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/Inventory/InventorySessions/Import/ConfirmInventoryImportCommandHandler.cs:207) looks up category, units, item, location, and price per row. [GetUnit](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/Inventory/InventorySessions/Import/ConfirmInventoryImportCommandHandler.cs:493) reloads the entire units table each time. Large imports hold a transaction while performing thousands of sequential queries.

Preload lookup tables once, retrieve relevant items/prices/locations in bounded batches, and use dictionaries inside the loop. Keep a single transaction for the business operation. Measure query count and duration using a realistic large workbook before choosing further bulk optimizations.

### 10. Medium — business changes and their audit records often commit separately

[DeleteBranchCommandHandler](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/Branches/Commands/DeleteBranch/DeleteBranchCommandHandler.cs:44), user changes, and financial approval save business changes before calling AuditLogService, which performs another SaveChanges. The registered [AuditLogInterceptor](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Infrastructure/Persistence/Interceptors/AuditLogInterceptor.cs:5) is empty.

An audit-write failure can leave a completed mutation reported as an error, with no corresponding audit record. Stage audit entries in the same unit of work/transaction or use a transactional outbox where appropriate. Do not rely on the interceptor registration as evidence of automatic auditing. The inventory confirmation transaction already demonstrates a better boundary.

### 11. Medium — unexpected exceptions lose diagnostic details

[ExceptionHandlingMiddleware](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.API/Middlewares/ExceptionHandlingMiddleware.cs:145) returns a generic 500 but never logs the exception. It also maps every InvalidOperationException to 400 and may expose its message, even when the exception represents a programming or infrastructure failure.

Log unexpected failures with structured context and a correlation ID; return a safe consistent error contract. Use explicit application exceptions/result types for expected validation/conflict failures. Handle request cancellation separately. Keep sensitive details out of responses and logs.

### 12. Medium — frontend authentication state stays stale after a 401

[apiClient](D:/Projects/InventorySystem/review-extracted/frontend/src/services/api/apiClient.js:58) clears localStorage, but [AuthContext](D:/Projects/InventorySystem/review-extracted/frontend/src/context/AuthContext.jsx:12) initializes its authentication state only once and updates it only through signIn/signOut. Clearing storage in the same tab does not update that React state.

An expired session can remain on a protected screen and repeatedly fail requests until a refresh or explicit logout. Publish a centralized session-expired event or inject an auth failure callback that calls signOut. Also align [LoginHandler's fixed eight-hour response expiry](D:/Projects/InventorySystem/review-extracted/src/InventorySystem.Application/Features/Authentication/Login/LoginHandler.cs:121) with JwtService's configurable expiration; compute both from one result.

### 13. Medium — parallel implementations make fixes easy to apply to unused code

The archive has 102 files under frontend/src/modules/inventory, while [AppRoutes](D:/Projects/InventorySystem/review-extracted/frontend/src/routes/AppRoutes.jsx:1) imports the top-level screens. It also has 22 files under the older Application/Features/InventorySessions tree in addition to Features/Inventory/InventorySessions, plus duplicate inventory export services in different namespaces. These are parallel implementations, not all byte-identical copies.

Choose one canonical inventory module, migrate imports/routes/DI explicitly, then remove superseded copies after reference checks. Two independent InventorySessionContext instances are particularly risky if old and new components get mixed. Remove the nested frontend/src package/config scaffold and empty placeholders if they have no intended role. Do not delete directories solely because their names look duplicated.

### 14. Medium — frontend lint quality gate currently fails

The 90 errors are measurable cleanup debt. [Arabic localization](D:/Projects/InventorySystem/review-extracted/frontend/src/localization/ar.js:100) contains duplicate keys, with analogous findings in English; later object properties overwrite earlier ones. Other findings include effect-driven state resets and unstable/manual memoization dependencies.

Remove duplicate keys deliberately, preserving the intended translations. Review effect findings by behavior instead of blanket-disabling rules. Make lint and production build required checks. Add focused UI tests for form reset behavior, filter changes, and session expiry.

## Architecture and MVC assessment

| Area | Assessment | Practical direction |
|---|---|---|
| Overall style | Layered modular application using CQRS/MediatR and a React SPA | Preserve the architecture; a server-rendered MVC conversion would not fix these defects. |
| Domain | Has no outward package/project dependencies, which is good; mostly mutable data entities | Move important state transitions and financial invariants into explicit domain methods/policies as complexity warrants. |
| Application | Depends on Domain, but also EF Core and ClosedXML; exposes DbSet and DatabaseFacade through IApplicationDbContext | This is pragmatic layering, not strict persistence independence. Keep EF deliberately if useful; move spreadsheet parsing behind an interface and avoid adding repositories just for ceremony. |
| Infrastructure | Owns EF configurations/migrations, authentication, storage, report rendering | Appropriate responsibilities. Extract report sections/layout helpers and separate business calculations from document rendering. |
| API/controllers | Generally delegates to MediatR and application services; Program is the composition root | Good foundation. Consolidate duplicate import routes and policies; move registrations into module extension methods. Referencing Infrastructure at composition time is expected. |
| React | Screens, components, services, contexts, and permission routes exist | Use one feature tree with shared UI, API contracts, and auth. Introduce route-level lazy loading where useful. |
| Shared | Empty exception/response placeholders, not referenced by application projects | Remove the empty project or give it a narrow, actively used responsibility; avoid a miscellaneous shared dumping ground. |
| Quality controls | Successful compilation but no discovered automated tests and failing lint | Add risk-based tests before large refactors, then enforce checks in CI. |

In this system, React renders the views, API controllers adapt HTTP requests, and the Application/Domain layers hold use cases and data rules. API controllers do not need Razor Views to be correctly structured. CQRS does not require separate databases, and MediatR registration alone does not enforce authorization or transaction boundaries.

Specific maintainability work:

- Split the 4,535-line FinancialDashboardExcelExportService, 2,322-line PDF exporter, 2,225-line period exporter, and 1,558-line FinancialExcelParser along report/file-type responsibilities. Long generated migration files are expected and should not be treated as the same smell.
- Replace JSON serialization/property-name manipulation in the aggregate dashboard handler with typed section contracts where practical. Frontend fallback chains for several possible DTO property names suggest contract drift; generate or explicitly maintain a stable API schema/client.
- Centralize financial branch eligibility (`0`, `001`, `002` exclusions appear in multiple handlers), expected-file rules, time conventions, and permission identifiers. Snapshot period requirements if historical reproducibility is required.
- Standardize namespace/folder alignment, `Interface` versus `Interfaces`, handler naming, status enums, and formatting. Remove obsolete commented implementations such as the old App.jsx auto-login block.
- Keep HTTP upload size limits, user-facing messages, extensions, and validation consistent across import endpoints. One inventory route defines 50 MB while reporting 10 MB. Financial uploads buffer the file before applying the command's 20 MB limit; enforce limits before making extra copies.
- Define deletion semantics deliberately: several entities contain IsDeleted, but master-data/user delete handlers physically remove records. Document which records must be retained and map relational conflicts to useful client errors.
- Package source separately from node_modules, build output, .git, real credentials, and uploaded operational spreadsheets. A 659,738,461-byte ZIP is unnecessarily large for source review and mixes concerns.

## Recommended order of work

1. Secure configuration, export permissions, token invalidation, and canonical import validation.
2. Repair financial upload/approval transactions and concurrency, category reuse, audit atomicity, and completeness validation.
3. Add regression tests for those scenarios using a disposable SQL Server database where relational behavior matters. Do not substitute only EF in-memory tests for unique indexes or transaction races.
4. Consolidate duplicate modules with reference checks and feature smoke tests. Preserve current behavior while moving files.
5. Refactor parsers/exporters, establish stable DTOs, batch database work, and make lint/build/tests required CI checks.

No automatic refactor or source fixes were applied in this review. Successful builds establish compilation/bundling only; they do not override the security and correctness findings above.

