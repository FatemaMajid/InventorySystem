using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Entity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Details)
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.Entity);
    }
}