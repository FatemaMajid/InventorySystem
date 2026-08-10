using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class InventorySessionConfiguration : IEntityTypeConfiguration<InventorySession>
{
    public void Configure(EntityTypeBuilder<InventorySession> builder)
    {
        builder.ToTable("InventorySessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SessionNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.SessionNumber)
            .IsUnique();

        builder.Property(x => x.InventoryType)
            .IsRequired();

        builder.Property(x => x.InventoryDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.BeforeFileName)
            .HasMaxLength(500);

        builder.Property(x => x.AfterFileName)
            .HasMaxLength(500);

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

        // Inventory Details
        builder.HasMany(x => x.Details)
            .WithOne(x => x.InventorySession)
            .HasForeignKey(x => x.InventorySessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Useful for filtering inventory history
        builder.HasIndex(x => new
        {
            x.BranchId,
            x.StoreId,
            x.InventoryDate
        });
    }
}