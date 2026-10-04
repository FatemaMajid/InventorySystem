using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class ItemLocationConfiguration
    : IEntityTypeConfiguration<ItemLocation>
{
    public void Configure(EntityTypeBuilder<ItemLocation> builder)
    {
        builder.ToTable("ItemLocations");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Item)
            .WithMany(x => x.ItemLocations)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ItemId,
            x.BranchId,
            x.StoreId
        })
        .IsUnique();
    }
}