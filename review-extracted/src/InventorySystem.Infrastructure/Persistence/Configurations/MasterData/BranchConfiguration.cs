using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BranchCode)
            .HasMaxLength(3)
            .IsRequired();
        builder.HasIndex(x => x.BranchCode)
            .IsUnique();

        builder.Property(x => x.BranchNameArabic)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.BranchNameEnglish)
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);                    
    }
}