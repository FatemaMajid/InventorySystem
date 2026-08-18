using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InventorySystem.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Store> Stores => Set<Store>();

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Unit> Units => Set<Unit>();

    public DbSet<Item> Items => Set<Item>();

    public DbSet<ItemLocation> ItemLocations => Set<ItemLocation>();

    public DbSet<ItemPrice> ItemPrices => Set<ItemPrice>();

    public DbSet<ItemPriceHistory> ItemPriceHistories
        => Set<ItemPriceHistory>();

    public DbSet<InventorySession> InventorySessions
        => Set<InventorySession>();

    public DbSet<InventoryDetail> InventoryDetails
        => Set<InventoryDetail>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<UserPermission> UserPermissions => Set<UserPermission>(); public new DatabaseFacade Database
            => base.Database;

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}