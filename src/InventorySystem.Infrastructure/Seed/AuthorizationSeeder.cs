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


    private static async Task SeedPermissionsAsync(
        ApplicationDbContext context)
    {
        var permissions = new[]
        {
            new Permission
            {
                Code = PermissionCodes.DashboardView,
                Name = "Dashboard View"
            },

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

            new Permission
            {
                Code = PermissionCodes.ComparisonView,
                Name = "Comparison Results View"
            },

            new Permission
            {
                Code = PermissionCodes.AttentionView,
                Name = "Attention Items View"
            },

            new Permission
            {
                Code = PermissionCodes.ReportView,
                Name = "Reports View"
            },

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

            new Permission
            {
                Code = PermissionCodes.AuditLogView,
                Name = "Audit Logs View"
            },

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
                    .AnyAsync(x =>
                        x.Code == permission.Code);

            if (!exists)
            {
                context.Permissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();
    }


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
                    "Inventory and user administration."
            },

            new Role
            {
                Name = "User",
                Description =
                    "View only access."
            }
        };


        foreach (var role in roles)
        {
            var exists =
                await context.Roles
                    .AnyAsync(x =>
                        x.Name == role.Name);

            if (!exists)
            {
                context.Roles.Add(role);
            }
        }

        await context.SaveChangesAsync();
    }


    private static async Task SeedRolePermissionsAsync(
        ApplicationDbContext context)
    {
        var manager =
            await context.Roles
                .FirstAsync(x =>
                    x.Name == "Manager");

        var admin =
            await context.Roles
                .FirstAsync(x =>
                    x.Name == "Admin");

        var user =
            await context.Roles
                .FirstAsync(x =>
                    x.Name == "User");


        var allPermissions =
            await context.Permissions
                .ToListAsync();


        // =====================================================
        // MANAGER
        // Full access
        // =====================================================

        foreach (var permission in allPermissions)
        {
            await AddRolePermissionIfMissing(
                context,
                manager.Id,
                permission.Id);
        }


        // =====================================================
        // ADMIN
        // =====================================================

        var adminPermissions = new[]
        {
            PermissionCodes.DashboardView,

            PermissionCodes.InventorySessionView,
            PermissionCodes.InventorySessionCreate,

            PermissionCodes.ComparisonView,
            PermissionCodes.AttentionView,
            PermissionCodes.ReportView,

            PermissionCodes.BranchView,
            PermissionCodes.StoreView,

            PermissionCodes.UserView,
            PermissionCodes.UserCreate,
            PermissionCodes.UserEdit,
            PermissionCodes.UserDeactivate,

            PermissionCodes.RoleView,
            PermissionCodes.RoleEdit
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


        // =====================================================
        // USER
        // View only
        // =====================================================

        var userPermissions = new[]
        {
            PermissionCodes.DashboardView,

            PermissionCodes.InventorySessionView,

            PermissionCodes.ComparisonView,

            PermissionCodes.AttentionView,

            PermissionCodes.ReportView,

            PermissionCodes.BranchView,

            PermissionCodes.StoreView
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


    private static async Task AddRolePermissionIfMissing(
        ApplicationDbContext context,
        int roleId,
        int permissionId)
    {
        var exists =
            await context.RolePermissions
                .AnyAsync(x =>
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