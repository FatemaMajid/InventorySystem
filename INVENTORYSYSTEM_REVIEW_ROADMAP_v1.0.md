# InventorySystem — Enterprise Full-Stack Review Roadmap

**Roadmap version:** 1.0 (2026-11-07)
**Status:** BASELINE — newly agreed plan; do not retroactively mark stages complete
**Source of scope:** uploaded `InventorySystem_Full_Code_Review.zip`, first-pass repository audit, and user's stated objective of a clean, secure, professional, maintainable backend/frontend before completing the Financial Analysis UI.

> **Important:** This is a **newly reconstructed and fixed 10-stage plan**, NOT a claim that the lost original ten-stage wording has been recovered. Once accepted, retain these IDs and names. Any scope change requires a recorded change log and user agreement.

## Completion rules — mandatory for every stage
1. Create a baseline inventory of the exact files and commit/working-tree state affected.
2. Produce a change list with reasons; delete only after verifying usages/imports and checking dynamic references.
3. Preserve existing business rules, DB data and API contracts unless an explicitly approved change is necessary.
4. Run relevant compile/build, lint, tests, and an appropriate runtime regression check for changed components.
5. Record evidence: command, result, key test counts, failures/limitations, and source change list.
6. Update this document: status, tasks, blockers, date, evidence, and next stage. Stage becomes `DONE` only after **every acceptance criterion** is satisfied. Never infer progress from number of chats/commands or tests alone.
7. Do not commit, push, migrate, or delete without reviewing the exact affected paths and intended environment; production data requires a backup, deployment plan, and confirmation.

**Status vocabulary:** `NOT STARTED`, `IN PROGRESS`, `BLOCKED`, `READY FOR REVIEW`, `DONE`.
**Progress metric:** verified DONE stages / 10 (equal-stage milestone count, **not** estimated implementation effort). For an in-progress stage report completed criteria out of total criteria; no invented percentage.
**Baseline verified DONE count on 2026-11-07:** **0/10** under this *new* plan. This does **not** mean earlier development or security work was lost; it means it has not yet met this plan's verification gates.

## The fixed ten stages

| ID | Stage | What is included | Acceptance evidence (minimum) | Baseline status |
|---|---|---|---|---|
| **01** | **Baseline, Inventory & Risk Register** | Map the solution, actual frontend working directory, active routes, dependency graph, duplicate/backup/release artifacts; record Git changes, deployed vs local environment; create issue register | Versioned inventory, active import map, deletion candidates, severity-ranked risk list, confirmed code baseline, documentation | **IN PROGRESS** |
| **02** | **Repository Structure & Dead-Code Cleanup** | Standardize folder/module boundaries, remove *verified* unused legacy components/backups/test scaffolds, resolve duplicate frontend Vite roots, ignore build artifacts | File-by-file keep/move/delete evidence; no broken imports; Git diff reviewed; clean build and smoke checks | NOT STARTED |
| **03** | **Backend Architecture & Code Quality** | Audit API/Application/Domain/Infrastructure boundaries; Controller/DTO/CQRS/validators/service responsibilities; dependency injection, logging, maintainability, formatting; split oversized files only when safe | Contract-preserving refactors, targeted tests, .NET full build, no unexplained behavior changes; compact C# style preserved | NOT STARTED |
| **04** | **Frontend Architecture & Code Quality** | React components/screens/modules, hooks/contexts, services, routing, permissions, performance, API error and download consistency, styles, reusable UI | Authoritative imports, eliminated verified duplicates, lint/build, navigation/auth smoke checks; RTL preserved | NOT STARTED |
| **05** | **Database Integrity & Financial Business Rules** | EF migrations/schema/indexes/constraints, branch mappings, financial periods, transaction logic, versioning/concurrent uploads, queries and performance, safe migrations | Isolated SQL integration tests including version clashes, rollback, cancellation, commit ambiguity; migration verified against intended environment; no data loss | NOT STARTED |
| **06** | **Security Audit & Remediation** | Authentication/JWT/revocation, authorization/IDOR, uploads, CORS/proxy, XSS/token storage, secrets/logging, error disclosure, rate limiting, dependency advisories | Endpoint-role/IDOR test matrix, documented mitigations, validated config with secrets redacted, high/critical findings resolved or explicitly accepted | NOT STARTED |
| **07** | **Localization, UX Contracts & Consistency** | All user-facing backend/frontend error messages Arabic/English, RTL, validation labels, API error contracts and number/date handling | Translation key coverage, AR/EN and RTL checks, no unintended hardcoded user-facing errors; contract tests | NOT STARTED |
| **08** | **Automated Testing & End-to-End Regression** | Unit, integration, SQL isolation, API contract, React UI and end-to-end flows; include inventory + financial cases, permission matrix | Recorded reproducible commands, green .NET and frontend build/lint/test suites, documented coverage/gaps and exploratory smoke checklist | NOT STARTED |
| **09** | **Financial Analysis UI Readiness & Implementation Handoff** | Validate existing API/DTO/service contracts, screens/workflows, design and reuse conventions; align Income/Position/Transfer/Settlement/periods/uploads with backend | Reviewed contract matrix, UI sitemap and component/service plan, agreed gaps fixed; ready to implement/customize Financial Analysis UI without structural rework | NOT STARTED |
| **10** | **Final Clean Audit, Documentation & Release Readiness** | Final regression and security spot-check, deployment/backup/monitoring configuration, repository hygiene, staged diff review, changelog, rollback plan | Final clean verification record, unresolved risk register, approved commit/push/deployment steps, roadmap + README updated | NOT STARTED |

## Verified work from the earlier security/hardening effort — **carry forward, not stage completion**
- Previous security sub-stages 1–5 had been reported complete; **not yet revalidated against this repository-wide roadmap**.
- Financial upload changes include path/size protections, localized Excel validation, and transaction/cancellation/conflict handling.
- `20261107065024_AddFinancialUploadSlotVersionUniqueIndex` was generated and applied successfully to the configured database; `dotnet ef migrations list` displayed it.
- Last reported full .NET solution build succeeded, and **21/21** xUnit tests passed; `git diff --check` succeeded.
- **Outstanding:** concurrency/fault-injection integration verification, IDOR/access review, production configuration inspection, React build/lint; these tests cannot be assumed to pass.
- The Git changes were staged but user had not confirmed a commit; `.bak` files remained untracked; no blind cleanup.

## Findings from first ZIP inspection to route into stages
- Parallel `frontend/src/screens/...` and `frontend/src/modules/inventory/...` implementations, mixed `AppRoutes` imports (01/02/04).
- Two Vite/package roots, source backups, `publish/` artifact in archive (01/02).
- Financial upload concurrency relies partly on identifying SQL index names via exception text; missing fault tests (05/06/08).
- Hardcoded English errors in Financial Upload controller/validator and frontend service (07).
- Oversized Excel/report exporters/parser; only refactor with golden-file tests (03/08).
- `localStorage` JWT/XSS tradeoff, role/IDOR test gaps, secret-bearing config not included in ZIP (06).
- Financial UI placeholder/unfinished workflows (09).

## Stage record template — copy/update at each closure
```text
Stage ID / fixed name:
Status:
Start date / closure date:
Baseline Git commit and working tree:
Scope/files inspected:
Changes made (paths + rationale):
Deleted files (proof of unused references):
Tests/build/lint/run and actual results:
Security/DB impact and rollback plan:
Known limitations and blockers:
Acceptance criteria met [N/N]:
Approval / commit hash (if applicable):
Next stage:
```

## Change log
| Date | Change | Reason |
|---|---|---|
| 2026-11-07 | New v1.0 roadmap; fixed stage IDs 01–10, evidence-based closure rule, initial status | Reconstruct a stable plan from user objective and repository audit after original plan's wording was lost |

**First action for Stage 01:** verify repository entrypoints and active imports; list actual version-controlled files, changed/staged files, and candidate duplicate/backup assets. Do not delete yet.

## Stage 02 Completion Record — 2026-11-07

Status: COMPLETED
Scope: Repository Structure & Dead-Code Cleanup

Verified:
- Legacy frontend screens and components moved to external quarantine.
- Six unused root Inventory services moved to quarantine.
- Seven frontend backup files moved to quarantine.
- Thirty-six backend backup files moved to quarantine.
- Frontend production build passed (288 modules).
- Backend solution build passed.
- Automated tests passed: 21/21, zero failures.
- Git diff --check passed.
- Runtime smoke test passed for principal Inventory and Financial Analysis pages.

Deferred:
- Full module architecture restructuring: Stage 04.
- Financial business-rule verification: Stage 05.
- Reports comparison endpoint performance: Stage 04/08.
- Financial period text encoding: Stage 07.
- Complete functional and E2E regression: Stage 08.
- Production release readiness: Stage 10.

Safety:
- Quarantined files preserved outside repository.
- No database changes performed during cleanup.
- No commit or push performed.
---

## Stage 03 - Official Closure
<!-- STAGE03_OFFICIAL_CLOSURE_20261108 -->

Status: COMPLETED WITH DOCUMENTED RISKS
Accepted: 2026-11-08
Next Stage: 04 - Frontend Architecture & Code Quality

Verification:
- API build: passed, 0 warnings, 0 errors
- Backend regression: 35 passed, 0 failed
- Git diff --check: passed

Completed:
- Financial upload validator safety
- Financial upload concurrency and failure tests
- Financial Excel parser cancellation checks
- Branch and store seeder preservation
- Existing-user Manager role restoration prevented
- Inventory import rollback cancellation safety
- Inventory transaction rollback integration test

Open risks:
- Duplicate InventorySessions feature trees (22 files)
- Financial upload orphan-file recovery
- Broader financial export output regression coverage
- Additional end-to-end and operational test coverage

Constraints:
- Preserve existing financial calculation rules
- Preserve inventory comparison business logic
- Do not modify production data without separate approval
- No commit or push performed by this closure
