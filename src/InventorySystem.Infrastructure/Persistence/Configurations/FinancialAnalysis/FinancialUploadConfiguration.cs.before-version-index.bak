using InventorySystem.Domain.FinancialAnalysis.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations.FinancialAnalysis;

public class FinancialUploadConfiguration : IEntityTypeConfiguration<FinancialUpload>
{
    public void Configure(EntityTypeBuilder<FinancialUpload> builder)
    {
        builder.ToTable("FinancialUploads");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileType)
            .IsRequired();

        builder.Property(x => x.Scope)
            .IsRequired();

        builder.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.StoredFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.StoragePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.FileHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.UploadedBy)
            .HasMaxLength(200);

        builder.Property(x => x.ValidationMessage)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.FinancialPeriodId,
            x.FileType,
            x.BranchId,
            x.Gender
        });

        builder.HasIndex(x => new
        {
            x.FinancialPeriodId,
            x.FileType,
            x.BranchId,
            x.Gender,
            x.IsActive
        });

        builder.HasIndex(x => x.FileHash);

        builder.HasIndex(x => new
        {
            x.FinancialPeriodId,
            x.FileType,
            x.BranchId,
            x.Gender
        })
        .IsUnique()
        .HasFilter("[IsActive] = 1");

        builder.HasMany(x => x.AccountRows)
            .WithOne(x => x.FinancialUpload)
            .HasForeignKey(x => x.FinancialUploadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TransferRecords)
            .WithOne(x => x.FinancialUpload)
            .HasForeignKey(x => x.FinancialUploadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SettlementSummary)
            .WithOne(x => x.FinancialUpload)
            .HasForeignKey<SettlementSummary>(x => x.FinancialUploadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.SettlementRows)
            .WithOne(x => x.FinancialUpload)
            .HasForeignKey(x => x.FinancialUploadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
