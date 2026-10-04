using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class InventoryDetailConfiguration
    : IEntityTypeConfiguration<InventoryDetail>
{
    public void Configure(EntityTypeBuilder<InventoryDetail> builder)
    {
        builder.ToTable("InventoryDetails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityBefore)
            .HasPrecision(18, 3);

        builder.Property(x => x.QuantityAfter)
            .HasPrecision(18, 3);

        builder.Property(x => x.QuantityDifference)
            .HasPrecision(18, 3);

        builder.Property(x => x.ConsumerPriceBefore)
            .HasPrecision(18, 2);

        builder.Property(x => x.ConsumerPriceAfter)
            .HasPrecision(18, 2);

        builder.Property(x => x.BeforeValue)
            .HasPrecision(18, 2);

        builder.Property(x => x.AfterValue)
            .HasPrecision(18, 2);

        builder.Property(x => x.ValueDifference)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne(x => x.InventorySession)
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.InventorySessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Item)
            .WithMany(x => x.InventoryDetails)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.InventorySessionId,
            x.ItemId
        })
        .IsUnique();
        builder.HasIndex(x => x.InventorySessionId);
    }
}