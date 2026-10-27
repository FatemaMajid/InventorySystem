using InventorySystem.Domain.FinancialAnalysis.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations.FinancialAnalysis;

public class SettlementRowConfiguration : IEntityTypeConfiguration<SettlementRow>
{
    public void Configure(EntityTypeBuilder<SettlementRow> builder)
    {
        builder.ToTable("SettlementRows");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.EntryNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Debit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Credit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Balance)
            .HasPrecision(18, 2);

        builder.Property(x => x.RowOrder)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.FinancialUploadId,
            x.RowOrder
        });

        builder.HasIndex(x => new
        {
            x.FinancialUploadId,
            x.TransactionDate
        });
    }
}
