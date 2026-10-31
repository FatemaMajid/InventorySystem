
using InventorySystem.Application.Common.Authorization;
using InventorySystem.Domain.Entities.Authorization;
using InventorySystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Seed;

public static class AuthorizationSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        await SeedPermissionsAsync(context);
        await SeedRolesAsync(context);
        await SeedRolePermissionsAsync(context);
    }

    // =====================================================
    // PERMISSIONS
    // =====================================================

    private static async Task SeedPermissionsAsync(
        ApplicationDbContext context)
    {
        var permissions = new[]
        {
            // Dashboard
            new Permission
            {
                Code = PermissionCodes.DashboardView,
                Name = "Dashboard View"
            },
            new Permission
            {
                Code = PermissionCodes.DashboardExport,
                Name = "Dashboard Export"
            },

            // Inventory Sessions
            new Permission
            {
                Code = PermissionCodes.InventorySessionView,
                Name = "Inventory Sessions View"
            },
            new Permission
            {
                Code = PermissionCodes.InventorySessionCreate,
                Name = "Inventory Session Create"
            },

            // Comparison
            new Permission
            {
                Code = PermissionCodes.ComparisonView,
                Name = "Comparison Results View"
            },
            new Permission
            {
                Code = PermissionCodes.ComparisonExport,
                Name = "Comparison Results Export"
            },

            // Attention Items
            new Permission
            {
                Code = PermissionCodes.AttentionView,
                Name = "Attention Items View"
            },
            new Permission
            {
                Code = PermissionCodes.AttentionExport,
                Name = "Attention Items Export"
            },

            // Reports
            new Permission
            {
                Code = PermissionCodes.ReportView,
                Name = "Reports View"
            },
            new Permission
            {
                Code = PermissionCodes.ReportCreate,
                Name = "Report Create"
            },
            new Permission
            {
                Code = PermissionCodes.ReportExport,
                Name = "Report Export"
            },

            // Branches
            new Permission
            {
                Code = PermissionCodes.BranchView,
                Name = "Branches View"
            },
            new Permission
            {
                Code = PermissionCodes.BranchCreate,
                Name = "Branch Create"
            },
            new Permission
            {
                Code = PermissionCodes.BranchEdit,
                Name = "Branch Edit"
            },

            // Stores
            new Permission
            {
                Code = PermissionCodes.StoreView,
                Name = "Stores View"
            },
            new Permission
            {
                Code = PermissionCodes.StoreCreate,
                Name = "Store Create"
            },
            new Permission
            {
                Code = PermissionCodes.StoreEdit,
                Name = "Store Edit"
            },

            // Users
            new Permission
            {
                Code = PermissionCodes.UserView,
                Name = "Users View"
            },
            new Permission
            {
                Code = PermissionCodes.UserCreate,
                Name = "User Create"
            },
            new Permission
            {
                Code = PermissionCodes.UserEdit,
                Name = "User Edit"
            },
            new Permission
            {
                Code = PermissionCodes.UserDeactivate,
                Name = "User Deactivate"
            },

            // Roles & Permissions
            new Permission
            {
                Code = PermissionCodes.RoleView,
                Name = "Roles & Permissions View"
            },
            new Permission
            {
                Code = PermissionCodes.RoleEdit,
                Name = "Roles & Permissions Edit"
            },

            // Audit Logs
            new Permission
            {
                Code = PermissionCodes.AuditLogView,
                Name = "Audit Logs View"
            },

            // Settings
            new Permission
            {
                Code = PermissionCodes.SettingsView,
                Name = "Settings View"
            },

            // Financial Periods
            new Permission
            {
                Code = PermissionCodes.FinancialPeriodView,
                Name = "Financial Period View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialPeriodCreate,
                Name = "Financial Period Create"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialPeriodApprove,
                Name = "Financial Period Approve"
            },

            // Financial Analysis
            new Permission
            {
                Code = PermissionCodes.FinancialAnalysisView,
                Name = "Financial Analysis View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialSalesView,
                Name = "Financial Sales View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialInventoryView,
                Name = "Financial Inventory View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialExpensesView,
                Name = "Financial Expenses View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialPurchasesView,
                Name = "Financial Purchases View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialProfitLossView,
                Name = "Financial Profit Loss View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialCustomersView,
                Name = "Financial Customers View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialDebtView,
                Name = "Financial Debt View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialCashView,
                Name = "Financial Cash View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialSuppliersView,
                Name = "Financial Suppliers View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialCapitalView,
                Name = "Financial Capital View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialTransfersView,
                Name = "Financial Transfers View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialSettlementsView,
                Name = "Financial Settlements View"
            },
            new Permission
            {
                Code = PermissionCodes.FinancialDashboardView,
                Name = "Financial Dashboard View"
            },

            // Home
            new Permission
            {
                Code = PermissionCodes.HomeView,
                Name = "Home View"
            }
        };

        foreach (var permission in permissions)
        {
            var exists = await context.Permissions
                .AnyAsync(x => x.Code == permission.Code);

            if (!exists)
            {
                context.Permissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();
    }

    // =====================================================
    // ROLES
    // =====================================================

    private static async Task SeedRolesAsync(
        ApplicationDbContext context)
    {
        var roles = new[]
        {
            new Role
            {
                Name = "Manager",
                Description = "Full system access."
            },
            new Role
            {
                Name = "Admin",
                Description = "Full operational access."
            },
            new Role
            {
                Name = "User",
                Description =
                    "View inventory sessions and results only."
            }
        };

        foreach (var role in roles) {
            var exists = await context.Roles
                    .AnyAsync(x => x.Name == role.Name);

            if (!exists) {
                context.Roles.Add(role);
            }
        }

        await context.SaveChangesAsync();
    }

    // =====================================================
    // ROLE PERMISSIONS
    // =====================================================

    private static async Task SeedRolePermissionsAsync(
        ApplicationDbContext context)
    {
        var manager = await context.Roles
            .FirstAsync(x => x.Name == "Manager");

        var admin = await context.Roles
            .FirstAsync(x => x.Name == "Admin");

        var user = await context.Roles
            .FirstAsync(x => x.Name == "User");

        var allPermissions = await context.Permissions
            .ToListAsync();

        var adminPermissions = new[]
        {
            PermissionCodes.HomeView,
            PermissionCodes.DashboardView,
            PermissionCodes.DashboardExport,
            PermissionCodes.InventorySessionView,
            PermissionCodes.InventorySessionCreate,
            PermissionCodes.ComparisonView,
            PermissionCodes.ComparisonExport,
            PermissionCodes.AttentionView,
            PermissionCodes.AttentionExport,
            PermissionCodes.ReportView,
            PermissionCodes.ReportCreate,
            PermissionCodes.ReportExport,
            PermissionCodes.BranchView,
            PermissionCodes.StoreView,
            PermissionCodes.FinancialPeriodView,
            PermissionCodes.FinancialPeriodCreate,
            PermissionCodes.FinancialPeriodApprove,

            // Financial Analysis
            PermissionCodes.FinancialAnalysisView,
            PermissionCodes.FinancialSalesView,
            PermissionCodes.FinancialInventoryView,
            PermissionCodes.FinancialExpensesView,
            PermissionCodes.FinancialPurchasesView,
            PermissionCodes.FinancialProfitLossView,
            PermissionCodes.FinancialCustomersView,
            PermissionCodes.FinancialDebtView,
            PermissionCodes.FinancialCashView,
            PermissionCodes.FinancialSuppliersView,
            PermissionCodes.FinancialCapitalView,
            PermissionCodes.FinancialTransfersView,
            PermissionCodes.FinancialSettlementsView,
            PermissionCodes.FinancialDashboardView
        };

        var userPermissions = new[]
        {
            PermissionCodes.InventorySessionView,
            PermissionCodes.ComparisonView,
            PermissionCodes.AttentionView,
            PermissionCodes.DashboardView,
            PermissionCodes.ReportView
        };

        // Manager: add missing permissions without removing existing ones.
        var managerPermissionIds = await context.RolePermissions
            .Where(x => x.RoleId == manager.Id)
            .Select(x => x.PermissionId)
            .ToListAsync();

        foreach (var permission in allPermissions)
        {
            if (!managerPermissionIds.Contains(permission.Id))
            {
                context.RolePermissions.Add(
                    new RolePermission
                    {
                        RoleId = manager.Id,
                        PermissionId = permission.Id
                    });
            }
        }

        // Admin: seed defaults only when the role has no permissions.
        // Existing permissions configured through the UI are preserved.
        var adminHasPermissions = await context.RolePermissions
            .AnyAsync(x => x.RoleId == admin.Id);

        if (!adminHasPermissions){
            var adminPermissionIds = allPermissions
                .Where(x => adminPermissions.Contains(x.Code))
                .Select(x => x.Id);

            foreach (var permissionId in adminPermissionIds) {
                context.RolePermissions.Add(new RolePermission{RoleId = admin.Id,PermissionId = permissionId});
            }
        }

        // User: seed defaults only when the role has no permissions.
        // Existing permissions configured through the UI are preserved.
        var userHasPermissions = await context.RolePermissions
            .AnyAsync(x => x.RoleId == user.Id);

        if (!userHasPermissions) {
            var userPermissionIds = allPermissions
                .Where(x => userPermissions.Contains(x.Code))
                .Select(x => x.Id);

            foreach (var permissionId in userPermissionIds) {
                context.RolePermissions.Add(new RolePermission{RoleId = user.Id,PermissionId = permissionId});
            }
        }

        // Do not delete direct user permissions during startup.
        await context.SaveChangesAsync();
    }
}
