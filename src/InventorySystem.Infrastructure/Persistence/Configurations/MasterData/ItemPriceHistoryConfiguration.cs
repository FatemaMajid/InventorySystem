using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class ItemPriceHistoryConfiguration : IEntityTypeConfiguration<ItemPriceHistory>
{
    public void Configure(EntityTypeBuilder<ItemPriceHistory> builder)
    {
        builder.ToTable("ItemPriceHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OldCustomerPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.NewCustomerPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.OldConsumerPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.NewConsumerPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ChangedAt)
            .IsRequired();

        builder.Property(x => x.ChangedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.ItemPrice)
            .WithMany(x => x.History)
            .HasForeignKey(x => x.ItemPriceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Useful for displaying price history efficiently
        builder.HasIndex(x => new
        {
            x.ItemPriceId,
            x.ChangedAt
        });
    }
}