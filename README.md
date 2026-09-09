# Inventory Management System

A web-based Inventory Management System designed to manage inventory sessions, compare inventory data, analyze differences, generate reports, and control access through role-based permissions.

## Overview

The Inventory Management System provides a complete inventory workflow from importing inventory data from Excel files to comparing inventory quantities and prices, analyzing differences, reviewing attention items, and generating reports.

The system also provides:

- User management
- Roles and permissions
- Branch management
- Store management
- Audit logging
- Arabic and English localization
- RTL and LTR layout support
- Light and dark themes

## Features

### Authentication & Authorization

- JWT-based user authentication.
- Role-based access control.
- Permission-based authorization.
- Protected application routes.
- Direct permissions for users.
- Active and inactive user accounts.

### Inventory Sessions

- Create inventory sessions.
- Support for Semi-Annual and Annual inventory types.
- Select a branch and store for each inventory session.
- Import Before Inventory Excel data.
- Import After Inventory Excel data.
- Validate imported Excel files.
- Compare inventory data using item codes.
- Calculate quantity differences.
- Calculate inventory value differences.
- Track price changes.
- Detect newly counted items.
- Detect fully depleted items.
- Detect items with undefined units.

### Dashboard

The Dashboard provides an overview of a selected inventory session, including:

- Inventory session information.
- Item status summary.
- Total items.
- Increased quantities.
- Decreased quantities.
- Matching items.
- Newly counted items.
- Fully depleted items.
- Items with price changes.
- Items with undefined units.
- Financial summary.
- Inventory value comparison.
- Items requiring attention.
- Excel export.
- PDF export.

### Reports

The Reports module provides detailed inventory comparison results, including:

- Item information.
- Quantity before inventory.
- Quantity after inventory.
- Quantity difference.
- Price before inventory.
- Price after inventory.
- Value before inventory.
- Value after inventory.
- Value difference.
- Item status.
- Attention-related information.
- Excel export.
- PDF export.

### Attention Items

The system identifies inventory items that require review, including:

- Newly counted items.
- Fully depleted items.
- Price changes.
- Undefined units.

### Branches

- Create branches.
- Edit branches.
- Delete branches.
- View branch information.
- Manage Arabic and English branch names.
- Manage branch codes.
- Manage branch address and phone number.

### Stores

- Create stores.
- Edit stores.
- Delete stores.
- View stores.
- Associate stores with branches.
- Manage Arabic and English store names.
- Manage store codes.

### Users

- Create users.
- Edit users.
- Delete users.
- Activate users.
- Deactivate users.
- Assign roles.
- Assign direct permissions.
- View user status.
- View user creation and update information.

### Roles & Permissions

The system provides role and permission management for the main application areas.

Supported permission areas include:

- Dashboard
- Inventory Sessions
- Attention Items
- Reports
- Branches
- Stores
- Users
- Roles & Permissions
- Audit Log
- Settings

Example permission codes:

```text
Dashboard.View
Dashboard.Export
InventorySession.View
InventorySession.Create
Attention.View
Report.View
Branch.View
Branch.Create
Branch.Edit
Store.View
Store.Create
Store.Edit
User.View
User.Create
User.Edit
User.Deactivate
Role.View
Role.Edit
AuditLog.View
Settings.View
```

### Audit Log

The Audit Log records important system and administrative actions.

Logged activities include:

- Login activity.
- Inventory session confirmation.
- Branch creation.
- Branch updates.
- Branch deletion.
- Store creation.
- Store updates.
- Store deletion.
- User creation.
- User updates.
- User deletion.
- Role management actions.

Audit records contain:

- User
- Action
- Entity
- Entity ID
- Status
- Details
- Date and time

### Localization

The application supports:

- English.
- Arabic.
- Automatic LTR layout for English.
- Automatic RTL layout for Arabic.
- Localized interface text.
- Localized date and time formatting.

### Theme

The application supports:

- Light mode.
- Dark mode.
- Persistent theme selection.

## Inventory Workflow

The main inventory workflow is:

1. Sign in to the system.
2. Select the inventory type.
3. Select the branch.
4. Select the store.
5. Enter the inventory session information.
6. Upload the Before Inventory Excel file.
7. Upload the After Inventory Excel file.
8. Validate the uploaded files.
9. Confirm the inventory session.
10. Compare inventory items using item codes.
11. Calculate quantity and value differences.
12. Review the Dashboard.
13. Review Reports.
14. Review Attention Items.

## Excel Import

The system supports Excel files with `.xlsx` and `.xls` extensions.

Inventory data may include:

- Item Code
- Item Name
- Category
- Quantity
- Price
- Unit

The system normalizes unit values during the import process.

Examples of normalized unit values:

```text
ق       → قطعة
قطعه    → قطعة
قطعةد   → قطعة
د       → درزن
سيت6ق   → سيت
```

If a unit is not defined in the system, the inventory process continues normally and the item can be identified as having an undefined unit.

An undefined unit does not stop the complete inventory session from being processed.

## Item Comparison

Items from the Before Inventory and After Inventory files are matched using their item codes.

The comparison can produce the following statuses:

```text
NoDifference
Increase
Decrease
BeforeOnly
AfterOnly
```

The system calculates:

- Quantity difference.
- Value before inventory.
- Value after inventory.
- Value difference.

## Architecture

The project is organized into separate frontend and backend layers.

```text
InventorySystem/
├── frontend/
├── src/
│   ├── InventorySystem.API/
│   ├── InventorySystem.Application/
│   ├── InventorySystem.Domain/
│   ├── InventorySystem.Infrastructure/
│   └── InventorySystem.Shared/
├── tests/
├── InventorySystem.slnx
└── README.md
```

## Frontend Architecture

The frontend is built using a component-based architecture.

```text
frontend/
└── src/
    ├── assets/
    ├── components/
    ├── context/
    ├── localization/
    ├── routes/
    ├── screens/
    ├── services/
    └── styles/
```

Major frontend areas include:

```text
components/
├── AuditLog/
├── Branches/
├── Layout/
├── RolesPermissions/
├── Settings/
├── Stores/
└── Users/
```

Each major screen is separated from its reusable UI components.

CSS Modules are used for component and screen styling.

## Backend Architecture

### InventorySystem.API

Responsible for:

- API controllers.
- Authentication configuration.
- Authorization.
- HTTP endpoints.
- Application startup.

### InventorySystem.Application

Contains:

- Commands.
- Queries.
- Command handlers.
- Query handlers.
- DTOs.
- Validation.
- Application interfaces.
- Business workflows.

### InventorySystem.Domain

Contains:

- Domain entities.
- Enums.
- Domain models.

### InventorySystem.Infrastructure

Contains:

- Entity Framework Core.
- Database context.
- Entity configurations.
- Database migrations.
- Authentication infrastructure.
- Audit logging.
- Persistence-related services.

### InventorySystem.Shared

Contains shared functionality used across the application.

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

Before running the project, make sure the following are installed:

- .NET SDK
- Node.js
- npm
- Microsoft SQL Server
- Git

## Running the Backend

From the project root:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

Run the API:

```bash
dotnet run --project src/InventorySystem.API
```

## Running the Frontend

Navigate to the frontend directory:

```bash
cd frontend
```

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm run dev
```

## Database

The application uses Microsoft SQL Server with Entity Framework Core.

Database migrations are located in:

```text
src/InventorySystem.Infrastructure/Migrations/
```

Apply database migrations using the configured Entity Framework Core workflow.

## Configuration

The backend requires environment-specific configuration for:

- SQL Server connection string.
- JWT configuration.
- Authentication settings.

Sensitive values must not be committed to the repository.

Do not commit:

```text
Passwords
JWT Secrets
Production Connection Strings
API Keys
Private Credentials
```

## Security

The application uses JWT authentication and permission-based authorization to protect application functionality.

Users can only access protected features when they have the required permissions.

## Development Principles

The project follows these principles:

- Separation of concerns.
- Component-based frontend architecture.
- Layered backend architecture.
- Permission-based authorization.
- Centralized localization.
- Reusable UI components.
- CSS Modules for component styling.
- Consistent application theme and design system.
- Database migrations through Entity Framework Core.

## Current Modules

The current application includes:

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

The main inventory management workflow and administration modules are implemented.

The project continues to undergo testing, UI refinement, and final integration review.

## License

This project is currently maintained as a private project.