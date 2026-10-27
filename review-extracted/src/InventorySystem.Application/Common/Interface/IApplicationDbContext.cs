using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Entities.Authorization;
using InventorySystem.Domain.FinancialAnalysis.Entities;
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

    DbSet<FinancialPeriod> FinancialPeriods { get; }

    DbSet<FinancialUpload> FinancialUploads { get; }

    DbSet<FinancialAccountRow> FinancialAccountRows { get; }

    DbSet<TransferRecord> TransferRecords { get; }

    DbSet<SettlementSummary> SettlementSummaries { get; }

    DbSet<SettlementRow> SettlementRows { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
