using InventorySystem.Domain.FinancialAnalysis.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations.FinancialAnalysis;

public class TransferRecordConfiguration : IEntityTypeConfiguration<TransferRecord>
{
    public void Configure(EntityTypeBuilder<TransferRecord> builder)
    {
        builder.ToTable("TransferRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Gender)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.RowOrder)
            .IsRequired();

        builder.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.FinancialUploadId,
            x.BranchId
        });

        builder.HasIndex(x => new
        {
            x.FinancialUploadId,
            x.RowOrder
        });
    }
}
