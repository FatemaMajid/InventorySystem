using InventorySystem.Domain.FinancialAnalysis.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations.FinancialAnalysis;

public class FinancialPeriodConfiguration
    : IEntityTypeConfiguration<FinancialPeriod>
{
    public void Configure(
        EntityTypeBuilder<FinancialPeriod> builder)
    {
        builder.ToTable("FinancialPeriods");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ApprovedBy)
            .HasMaxLength(200);

        builder.HasIndex(x => x.StartDate);

        builder.HasIndex(x => x.EndDate);

        builder.HasMany(x => x.FinancialUploads)
            .WithOne(x => x.FinancialPeriod)
            .HasForeignKey(x => x.FinancialPeriodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
