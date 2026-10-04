# Inventory Module

This folder contains the existing inventory-counting application features.

- `Home`: inventory home/dashboard summary.
- `InventorySessions`: sessions, import, dashboard/comparison queries and commands.

Shared features such as Authentication, Branches, Stores, Users, Roles and AuditLogs remain outside this module because they are used across the system and can also be reused by Financial Analysis.

Phase 2 is an organizational refactor only. Existing API routes, database schema and inventory business rules are intentionally unchanged.
