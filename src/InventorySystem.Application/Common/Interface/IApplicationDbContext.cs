using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InventorySystem.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Store> Stores { get; }

    DbSet<Branch> Branches { get; }

    DbSet<Category> Categories { get; }

    DbSet<Unit> Units { get; }

    DbSet<Item> Items { get; }

    DbSet<ItemLocation> ItemLocations { get; }

    DbSet<ItemPrice> ItemPrices { get; }

    DbSet<ItemPriceHistory> ItemPriceHistories { get; }

    DbSet<InventorySession> InventorySessions { get; }

    DbSet<InventoryDetail> InventoryDetails { get; }

    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Permission> Permissions { get; }

    DbSet<UserRole> UserRoles { get; }

    DbSet<RolePermission> RolePermissions { get; }

    DbSet<UserPermission> UserPermissions { get; }

    DbSet<AuditLog> AuditLogs { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}