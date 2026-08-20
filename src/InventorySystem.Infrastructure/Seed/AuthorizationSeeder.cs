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

            new Permission
            {
                Code = PermissionCodes.ComparisonExport,
                Name = "Comparison Export"
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
            }
        };


        foreach (var permission in permissions)
        {
            var exists =
                await context.Permissions
                    .AnyAsync(
                        x => x.Code == permission.Code);

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
                Description =
                    "Full system access."
            },

            new Role
            {
                Name = "Admin",
                Description =
                    "Full operational and user administration access."
            },

            new Role
            {
                Name = "User",
                Description =
                    "View inventory sessions and results only."
            }
        };


        foreach (var role in roles)
        {
            var exists =
                await context.Roles
                    .AnyAsync(
                        x => x.Name == role.Name);

            if (!exists)
            {
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
        var manager =
            await context.Roles
                .FirstAsync(
                    x => x.Name == "Manager");

        var admin =
            await context.Roles
                .FirstAsync(
                    x => x.Name == "Admin");

        var user =
            await context.Roles
                .FirstAsync(
                    x => x.Name == "User");


        var allPermissions =
            await context.Permissions
                .ToListAsync();


        // =================================================
        // MANAGER
        // Full access
        // =================================================

        foreach (var permission in allPermissions)
        {
            await AddRolePermissionIfMissing(
                context,
                manager.Id,
                permission.Id);
        }


        // =================================================
        // ADMIN
        // Operational + User Administration
        // =================================================

        var adminPermissions = new[]
        {
            // Dashboard
            PermissionCodes.DashboardView,
            PermissionCodes.DashboardExport,


            // Inventory
            PermissionCodes.InventorySessionView,
            PermissionCodes.InventorySessionCreate,


            // Results
            PermissionCodes.ComparisonView,
            PermissionCodes.ComparisonExport,

            PermissionCodes.AttentionView,
            PermissionCodes.AttentionExport,


            // Reports
            PermissionCodes.ReportView,
            PermissionCodes.ReportCreate,
            PermissionCodes.ReportExport,


            // Branches - View ONLY
            PermissionCodes.BranchView,


            // Stores - View ONLY
            PermissionCodes.StoreView,


            // Users
            PermissionCodes.UserView,
            PermissionCodes.UserCreate,
            PermissionCodes.UserEdit,
            PermissionCodes.UserDeactivate,


            // Roles & Permissions
            PermissionCodes.RoleView,
            PermissionCodes.RoleEdit,
        };


        foreach (var code in adminPermissions)
        {
            var permission =
                allPermissions.First(
                    x => x.Code == code);

            await AddRolePermissionIfMissing(
                context,
                admin.Id,
                permission.Id);
        }


        // =================================================
        // USER
        // View inventory sessions and results ONLY
        // =================================================

        var userPermissions = new[]
        {
            PermissionCodes.InventorySessionView,

            PermissionCodes.ComparisonView,

            PermissionCodes.AttentionView,

            PermissionCodes.DashboardView
        };


        foreach (var code in userPermissions)
        {
            var permission =
                allPermissions.First(
                    x => x.Code == code);

            await AddRolePermissionIfMissing(
                context,
                user.Id,
                permission.Id);
        }


        await context.SaveChangesAsync();
    }


    // =====================================================
    // ADD ROLE PERMISSION IF MISSING
    // =====================================================

    private static async Task AddRolePermissionIfMissing(
        ApplicationDbContext context,
        int roleId,
        int permissionId)
    {
        var exists =
            await context.RolePermissions
                .AnyAsync(
                    x =>
                        x.RoleId == roleId &&
                        x.PermissionId == permissionId);

        if (!exists)
        {
            context.RolePermissions.Add(
                new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
        }
    }
}