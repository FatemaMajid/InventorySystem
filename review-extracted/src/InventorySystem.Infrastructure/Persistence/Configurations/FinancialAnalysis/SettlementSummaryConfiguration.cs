using InventorySystem.Domain.FinancialAnalysis.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations.FinancialAnalysis;

public class SettlementSummaryConfiguration : IEntityTypeConfiguration<SettlementSummary>
{
    public void Configure(EntityTypeBuilder<SettlementSummary> builder)
    {
        builder.ToTable("SettlementSummaries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PeriodDebit)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.PeriodCredit)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.PeriodBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasIndex(x => x.FinancialUploadId)
            .IsUnique();
    }
}
