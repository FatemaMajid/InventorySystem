using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application.Common.Interfaces;

namespace InventorySystem.Infrastructure.Persistence.Contexts;
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
    {
    }

    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Branch> Branches => Set<Branch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}