using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class ItemLocationConfiguration : IEntityTypeConfiguration<ItemLocation>
{
    public void Configure(EntityTypeBuilder<ItemLocation> builder)
    {
        builder.ToTable("ItemLocations");

        builder.HasKey(x => x.Id);

        // Item
        builder.HasOne(x => x.Item)
            .WithMany(x => x.ItemLocations)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Branch
        builder.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Store
        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Same item cannot be registered twice
        // in the same branch and store.
        builder.HasIndex(x => new
        {
            x.ItemId,
            x.BranchId,
            x.StoreId
        })
        .IsUnique();
    }
}