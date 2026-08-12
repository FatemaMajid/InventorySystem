using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ItemName1)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.ItemName2)
            .IsRequired(false)
            .HasMaxLength(300);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Item Code must be unique
        builder.HasIndex(x => x.ItemCode)
            .IsUnique();

        // Category relationship
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unit relationship
        builder.HasOne(x => x.Unit)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // Item Locations
        builder.HasMany(x => x.ItemLocations)
            .WithOne(x => x.Item)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // // Price
        // builder.HasMany(x => x.PriceHistory)
        //     .WithOne(x => x.ItemPrice)
        //     .HasForeignKey(x => x.ItemPriceId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // Inventory Details
        builder.HasMany(x => x.InventoryDetails)
            .WithOne(x => x.Item)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}