using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class InventoryDetailConfiguration : IEntityTypeConfiguration<InventoryDetail>
{
    public void Configure(EntityTypeBuilder<InventoryDetail> builder)
    {
        builder.ToTable("InventoryDetails");

        builder.HasKey(x => x.Id);

        // Quantities
        builder.Property(x => x.QuantityBefore)
            .HasPrecision(18, 3);

        builder.Property(x => x.QuantityAfter)
            .HasPrecision(18, 3);

        builder.Property(x => x.QuantityDifference)
            .HasPrecision(18, 3);

        // Consumer Prices
        builder.Property(x => x.ConsumerPriceBefore)
            .HasPrecision(18, 2);

        builder.Property(x => x.ConsumerPriceAfter)
            .HasPrecision(18, 2);

        // Values
        builder.Property(x => x.BeforeValue)
            .HasPrecision(18, 2);

        builder.Property(x => x.AfterValue)
            .HasPrecision(18, 2);

        builder.Property(x => x.ValueDifference)
            .HasPrecision(18, 2);

        // Comparison Status
        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Inventory Session
        builder.HasOne(x => x.InventorySession)
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.InventorySessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Item
        builder.HasOne(x => x.Item)
            .WithMany(x => x.InventoryDetails)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // One item appears once in each inventory session
        builder.HasIndex(x => new
        {
            x.InventorySessionId,
            x.ItemId
        })
        .IsUnique();
    }
}