using InventorySystem.Domain.Entities;
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

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}