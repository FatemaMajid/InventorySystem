# InventorySystem — Production Documentation

**Release:** v1.0  
**Status:** Production Stable  
**Documentation date:** 2026-09-19

---

## 1. Project Overview

InventorySystem is a production inventory-management application with a React/Vite frontend and an ASP.NET Core backend using SQL Server.

### Technology Stack

| Layer | Technology |
|---|---|
| Frontend | React + Vite |
| Frontend language | JavaScript / JSX |
| Frontend styling | CSS Modules / SVG icons |
| Backend | ASP.NET Core |
| Architecture | Clean Architecture |
| Application patterns | MediatR, FluentValidation, AutoMapper |
| ORM | Entity Framework Core |
| Database | Microsoft SQL Server |
| Authentication | JWT Bearer |
| Authorization | Role + permission-based authorization |
| Production hosting | SmarterASP.NET |
| Production OS/runtime | Windows / IIS / ASP.NET Core |
| Localization | Arabic / English |
| Database migrations | EF Core migrations |

---

## 2. Application Areas

The current application includes:

- Login / authentication
- Home
- Dashboard
- Inventory Sessions
- New Inventory Session
- Inventory Excel upload/import
- Comparison Results
- Attention Items
- Reports
- Branches
- Stores
- Users
- Roles & Permissions
- Audit Logs

The frontend uses protected routes and permission-aware UI controls. Backend endpoints independently enforce authorization.

---

## 3. Roles and Access Model

### Manager

Full system access.

### Administrator

Operational and administrative access according to the configured role permissions, including inventory operations, reports, branches/stores, and user/role administration as configured by the authorization seed.

### User

Restricted operational access. The current intended access includes:

- Home
- Dashboard.View
- InventorySession.View
- Comparison.View
- Attention.View

The User role must not access:

- Branches
- Stores
- Users
- Roles & Permissions
- Audit Logs
- Settings

### Important Authorization Rule

Frontend visibility is not the security boundary. Backend controllers use permission policies such as:

- Dashboard.View
- InventorySession.View
- InventorySession.Create
- Report.View
- Branch.View
- Store.View
- User.View
- User.Create
- User.Edit
- User.Deactivate
- Role.View
- Role.Edit
- AuditLog.View

JWT tokens contain role and permission claims.

---

## 4. Authentication

Authentication uses JWT Bearer tokens.

JWT validation includes:

- Issuer validation
- Audience validation
- Lifetime validation
- Signing-key validation
- One-minute clock skew

The frontend stores the authentication token locally and sends it as a Bearer token to the API.

### Token Expiration

The application uses an 8-hour expiration configuration in the current login flow.

### Important

Never place the JWT signing secret in frontend source code or commit it to a public repository.

---

## 5. Backend Security

### Rate Limiting

Login requests are rate-limited:

- 5 attempts
- Sliding window
- 1 minute
- Queue limit: 0

Rejected requests return HTTP 429.

### Security Headers

Current production security headers include:

- Strict-Transport-Security
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY
- Referrer-Policy: strict-origin-when-cross-origin
- Permissions-Policy

Content-Security-Policy was reviewed as a possible hardening improvement but was not added to the current stable production release.

### Swagger

Swagger is enabled only in Development.

Production Swagger endpoint was tested and returned 404.

### Exception Handling

Unhandled backend exceptions are processed by the exception middleware and do not expose raw stack traces or internal database details to clients.

---

## 6. Database

### Current Production Database

The application currently uses Microsoft SQL Server hosted with the production provider.

Production connection information is intentionally NOT stored in this document.

Keep the following as private deployment secrets:

- SQL Server hostname
- Database name
- Database username
- Database password
- Connection string
- JWT secret
- Seed Manager password

### Migrations

The production database was successfully updated using EF Core migrations.

For a new environment:

1. Configure the new database connection.
2. Verify the database is reachable.
3. Run the EF Core migrations.
4. Start the API.
5. Verify the seed process.
6. Test login and permissions.

Example:

```powershell
dotnet ef database update --project .\src\InventorySystem.API
```

Use the project's actual solution path if it differs.

---

## 7. Database Seeding

Startup seeding currently covers:

- Branch master data
- Store master data
- Unit master data
- Roles & permissions
- Initial Manager user

The Manager seeder reads its credentials from configuration.

Never document or commit the real seed password.

---

## 8. Production Deployment

### Current Hosting

The production deployment is on SmarterASP.NET.

The current deployment uses a Windows/IIS hosting environment with ASP.NET Core and SQL Server support.

### Production URL

The current temporary production URL is:

`https://fatemamajid2002-001-site1.dtempurl.com`

If the provider assigns a permanent domain later, replace this value in this document.

### Current Deployment Architecture

The intended production structure is:

```text
HTTPS Domain
    |
    +-- React/Vite frontend
    |
    +-- /api/... -> ASP.NET Core API
    |
    +-- SQL Server database
```

The production setup uses the same domain for frontend and API routing, which avoids the need for a public cross-origin frontend/API relationship.

---

## 9. Configuration When Moving Hosting Providers

The application code should remain mostly unchanged when moving providers.

Review these environment-specific items:

### Must be updated

- Database connection string
- JWT secret if required by the new environment
- Seed Manager configuration if required
- Frontend API base URL if the domain changes
- CORS origins if frontend and API become separate origins
- IIS/web.config or equivalent hosting configuration
- HTTPS configuration
- Publish profile / deployment method

### Usually unchanged

- Controllers
- Application handlers
- Domain entities
- EF Core models
- Authorization policies
- Frontend screens
- React components
- Business logic
- Validation rules

---

## 10. Production Backup

A complete post-production project backup was created after the latest changes.

Recommended naming:

```text
InventorySystem_Production_2026-09-19
```

Keep this version immutable as the stable production backup.

Do not use the stable backup as the working branch for new features.

Create a separate development copy for future changes.

---

## 11. Security Testing Completed

The following production security checks were completed.

| Test | Result |
|---|---|
| HTTPS / SSL | PASS |
| Security headers | PASS |
| X-Powered-By removal | PASS |
| Unauthenticated API access | PASS — 401 |
| Authorization / protected sections | PASS |
| XSS search tests | PASS |
| Stored XSS test on Branches | PASS |
| SQL Injection tests | PASS |
| IDOR / direct authorization tests | PASS |
| Login rate limiting | PASS — 429 |
| CORS cross-origin test | PASS — browser blocked |
| Unsupported PATCH method | PASS — 405 |
| Unauthorized DELETE test | Blocked |
| Error information disclosure | PASS — 404 without internal details |
| JWT tampering | PASS — 401 |
| API access without token | PASS — 401 |
| Swagger exposure | PASS — 404 |
| Frontend source maps | PASS — 404 |
| Sensitive configuration files | PASS — 404 |
| Production functional testing | PASS |

### Sensitive Files Tested

- `/.env`
- `/appsettings.json`
- `/appsettings.Development.json`
- `/web.config`
- `/robots.txt`

The first four returned 404 in production. `robots.txt` also returned 404, which is not a security problem.

---

## 12. Functional Testing Completed

The application was functionally tested using:

- Manager account
- Administrator account
- User account

Testing included:

- Login
- Main screens
- Permissions
- Inventory workflows
- Excel file upload
- Reports
- Comparison
- Administrative screens
- Role/permission behavior

The tested workflows were reported as working correctly.

---

## 13. Frontend Security Model

The frontend uses:

- Protected routes
- Permission-aware routes
- Permission-aware buttons/actions
- JWT-based API authorization
- Central API client
- Automatic handling of unauthorized sessions
- Localized Arabic/English UI

Frontend permissions are used for UX and visibility, while backend authorization remains authoritative.

---

## 14. Important Frontend Behavior

### Branch ordering

Where branches are shown in frontend lists/dropdowns/tables, they are ordered by `BranchCode` ascending according to the project's current behavior.

### Inventory session activity

The Home/Inventory Sessions statistics currently treat sessions from the last **15 days** as active according to the current frontend logic.

### Excel upload

The inventory upload currently accepts `.xlsx` files and enforces a maximum file size of **50 MB**.

---

## 15. Development / Release Workflow

Recommended workflow:

```text
Stable Production Backup
        |
        +-- Development Copy
                |
                +-- Feature / Fix
                |
                +-- Local Build
                |
                +-- Functional Test
                |
                +-- Security Regression Test if relevant
                |
                +-- Publish
                |
                +-- Production Verification
                |
                +-- New Stable Backup
```

Do not develop directly against the only production backup.

---

## 16. Release Checklist

Before a future production release:

### Code

- [ ] Build succeeds
- [ ] No compile errors
- [ ] No accidental debug code
- [ ] No secrets in frontend
- [ ] No secrets committed to source control

### Database

- [ ] Backup available
- [ ] Migrations reviewed
- [ ] Production connection verified
- [ ] Seed behavior reviewed

### Security

- [ ] HTTPS works
- [ ] Authentication works
- [ ] Authorization works
- [ ] Rate limiting works
- [ ] Security headers present
- [ ] Swagger disabled in production
- [ ] Sensitive files unavailable
- [ ] Source maps unavailable
- [ ] No new XSS/SQL injection paths introduced

### Functional

- [ ] Manager tested
- [ ] Administrator tested
- [ ] User tested
- [ ] Excel upload tested
- [ ] Reports tested
- [ ] Export functions tested
- [ ] CRUD operations tested
- [ ] Permissions tested

### Deployment

- [ ] Frontend published
- [ ] Backend published
- [ ] Configuration updated
- [ ] Database migrated
- [ ] HTTPS verified
- [ ] Production smoke test completed
- [ ] New backup created

---

## 17. Troubleshooting

### API does not start

Check:

- `Jwt:Secret`
- Database connection
- SQL Server availability
- .NET runtime compatibility
- IIS/ASP.NET Core hosting configuration

### Login fails after permission changes

Permissions are included in the JWT. Log out and log in again so a new token is issued.

### Frontend cannot reach API after moving hosts

Check:

1. API base URL.
2. HTTPS scheme.
3. CORS if frontend/API use different origins.
4. API route prefix `/api`.
5. IIS reverse-proxy/routing configuration.

### Database is empty after migration

Check:

- Migration status
- Connection string
- Seeder execution
- Database selected by the API

### Production returns unexpected 401

Check:

- Token exists
- Token has not expired
- Issuer
- Audience
- JWT secret
- Server time
- Permission claims

---

## 18. Future Hosting Migration

When moving to another provider, do not rebuild the application from scratch.

Use the stable backup and follow:

1. Create the new SQL Server database.
2. Configure the new connection string securely.
3. Run EF Core migrations.
4. Configure JWT secret and other environment secrets.
5. Publish the ASP.NET Core API.
6. Publish the React/Vite frontend.
7. Configure the API base URL.
8. Configure CORS only if the frontend/API are on different origins.
9. Configure HTTPS.
10. Configure IIS or the new provider's ASP.NET Core hosting.
11. Run the production smoke test.
12. Run the security checklist relevant to the new environment.
13. Create a new stable backup.

---

## 19. Current Release Status

**InventorySystem v1.0 — Production Stable**

The current production release has completed functional testing and the documented security checks above.

The current stable backup should remain unchanged. Future development should start from a separate working copy and produce a new release after testing.

---

## 20. Security Note

This document intentionally excludes real passwords, JWT secrets, database passwords, and other production credentials.

Never store those credentials in public documentation, screenshots, Git repositories, or frontend code.
