# Inventory Management System

A web-based Inventory Management System for managing inventory sessions, importing Excel data, comparing inventory quantities and values, analyzing differences, generating reports, and controlling access through role-based permissions.

## Overview

The system provides a complete inventory workflow:

1. Sign in with authenticated user access.
2. Select inventory type, branch, and store.
3. Create an inventory session.
4. Import Before Inventory and After Inventory Excel files.
5. Validate and process inventory data.
6. Compare items using item codes.
7. Calculate quantity and value differences.
8. Review Dashboard, Reports, and Attention Items.

The system also includes administration features such as:

- User management
- Roles and permissions
- Branch management
- Store management
- Audit logging
- Arabic and English localization
- RTL and LTR support
- Light and dark themes

## Main Features

### Authentication & Authorization

- JWT-based authentication
- Role-based access control
- Permission-based authorization
- Protected frontend routes
- Direct user permissions
- Active and inactive user accounts
- Login rate limiting

### Inventory Sessions

- Semi-Annual and Annual inventory types
- Branch and store selection
- Before Inventory Excel import
- After Inventory Excel import
- Excel validation
- Item-code based comparison
- Quantity difference calculation
- Inventory value difference calculation
- Price change detection
- Newly counted item detection
- Fully depleted item detection
- Undefined unit detection

### Dashboard

Provides an overview of an inventory session, including:

- Inventory session information
- Item status summary
- Quantity changes
- Matching items
- Newly counted items
- Fully depleted items
- Price changes
- Undefined units
- Financial summary
- Inventory value comparison
- Attention items
- Export options

### Reports

Detailed inventory comparison results including:

- Item information
- Before and after quantities
- Quantity differences
- Before and after prices
- Before and after values
- Value differences
- Item status
- Attention information
- Export options

### Administration

- Branch management
- Store management
- User management
- Roles and permissions
- Audit log
- Settings

## Inventory Comparison

Items from Before Inventory and After Inventory are matched using their item codes.

Possible comparison statuses:

```text
NoDifference
Increase
Decrease
BeforeOnly
AfterOnly
```

The system calculates:

- Quantity difference
- Value before inventory
- Value after inventory
- Value difference

## Architecture

```text
InventorySystem/
├── frontend/
│   └── src/
├── src/
│   ├── InventorySystem.API/
│   ├── InventorySystem.Application/
│   ├── InventorySystem.Domain/
│   ├── InventorySystem.Infrastructure/
│   └── InventorySystem.Shared/
├── database/
├── docs/
├── scripts/
└── InventorySystem.slnx
```

### Backend

The backend follows a layered/Clean Architecture approach:

- **API** — Controllers, authentication, authorization, HTTP endpoints, startup
- **Application** — Commands, queries, handlers, DTOs, validation, business workflows
- **Domain** — Entities, enums, and domain models
- **Infrastructure** — EF Core, database context, migrations, persistence, audit logging
- **Shared** — Shared application functionality

### Frontend

The frontend uses a component-based architecture:

```text
frontend/src/
├── assets/
├── components/
├── context/
├── localization/
├── routes/
├── screens/
├── services/
└── styles/
```

CSS Modules are used for component and screen styling.

## Technology Stack

### Frontend

- React
- JavaScript
- Vite
- React Router
- CSS Modules

### Backend

- C#
- ASP.NET Core
- Entity Framework Core
- MediatR
- FluentValidation
- JWT Authentication

### Database

- Microsoft SQL Server
- Entity Framework Core Migrations

## Requirements

- .NET SDK
- Node.js
- npm
- Microsoft SQL Server
- Git

## Running Locally

### Backend

From the project root:

```bash
dotnet restore
dotnet build
dotnet run --project src/InventorySystem.API
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

## Database

Database migrations are maintained with Entity Framework Core.

```text
src/InventorySystem.Infrastructure/Migrations/
```

Apply migrations using the configured Entity Framework Core workflow.

## Configuration

Environment-specific configuration is required for:

- SQL Server connection string
- JWT configuration
- Authentication settings

Sensitive values must never be committed to the repository.

Do not commit:

```text
Passwords
JWT Secrets
Production Connection Strings
API Keys
Private Credentials
```

## Security

The application uses multiple layers of protection, including:

- JWT authentication
- Permission-based authorization
- Protected frontend routes
- Login rate limiting
- HTTPS in production
- Security response headers
- Input validation
- Controlled error responses
- Production Swagger disabled

Security testing has been performed for authentication, authorization, XSS, SQL injection, IDOR, JWT tampering, rate limiting, error disclosure, and sensitive-file exposure.

## Localization & UI

The application supports:

- English
- Arabic
- Automatic LTR layout for English
- Automatic RTL layout for Arabic
- Localized interface text
- Localized date and time formatting
- Light mode
- Dark mode
- Persistent theme selection

## Current Modules

- Home
- Dashboard
- Inventory Sessions
- New Inventory Session
- Reports
- Attention Items
- Branches
- Stores
- Users
- Roles & Permissions
- Audit Log
- Settings

## Project Status

**InventorySystem v1.0 — Production Stable**

The current release represents the stable production version of the system.

The project is maintained as a private repository.

## Documentation

Detailed production documentation is maintained separately in the project archive:

```text
InventorySystem_Production_Documentation_v1.0.docx
InventorySystem_Production_Documentation_v1.0.md
```

## License

This project is currently maintained as a private project.
