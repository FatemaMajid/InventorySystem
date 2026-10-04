using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class ItemPriceConfiguration : IEntityTypeConfiguration<ItemPrice>
{
    public void Configure(EntityTypeBuilder<ItemPrice> builder)
    {
        builder.ToTable("ItemPrices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ConsumerPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        // One Item has one current price record
        builder.HasOne(x => x.Item)
            .WithOne()
            .HasForeignKey<ItemPrice>(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Price History
        builder.HasMany(x => x.History)
            .WithOne(x => x.ItemPrice)
            .HasForeignKey(x => x.ItemPriceId)
            .OnDelete(DeleteBehavior.Restrict);

        // One current price record per Item
        builder.HasIndex(x => x.ItemId)
            .IsUnique();
    }
}