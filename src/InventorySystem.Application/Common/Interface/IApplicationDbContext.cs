using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Branch> Branches { get; }

    DbSet<Store> Stores { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}