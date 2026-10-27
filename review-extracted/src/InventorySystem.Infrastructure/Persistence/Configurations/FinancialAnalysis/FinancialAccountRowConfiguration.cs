using InventorySystem.Domain.FinancialAnalysis.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations.FinancialAnalysis;

public class FinancialAccountRowConfiguration : IEntityTypeConfiguration<FinancialAccountRow>
{
    public void Configure(EntityTypeBuilder<FinancialAccountRow> builder)
    {
        builder.ToTable("FinancialAccountRows");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.AccountName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Debit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Credit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Balance)
            .HasPrecision(18, 2);

        builder.Property(x => x.Assets)
            .HasPrecision(18, 2);

        builder.Property(x => x.Liabilities)
            .HasPrecision(18, 2);

        builder.Property(x => x.RowOrder)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.FinancialUploadId,
            x.AccountCode
        });

        builder.HasIndex(x => new
        {
            x.FinancialUploadId,
            x.RowOrder
        });
    }
}
