using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.UnitNameArabic)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UnitNameEnglish)
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.UnitCode)
            .IsUnique();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Unit)
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}