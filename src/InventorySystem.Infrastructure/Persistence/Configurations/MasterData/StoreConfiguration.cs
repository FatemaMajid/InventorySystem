using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;
public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores");

        builder.HasKey(x => x.Id);   

        builder.Property(x => x.StoreCode)
               .HasMaxLength(3)
               .IsRequired();

        builder.Property(x => x.StoreNameArabic)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.StoreNameEnglish)
               .HasMaxLength(100);

        builder.Property(x => x.BranchCode)
               .HasMaxLength(3)
               .IsRequired();   

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);  

        builder.HasOne(x => x.Branch)
               .WithMany(x => x.Stores)
               .HasForeignKey(x => x.BranchCode)
               .HasPrincipalKey(x => x.BranchCode)
               .OnDelete(DeleteBehavior.Restrict);                                   
    }
}