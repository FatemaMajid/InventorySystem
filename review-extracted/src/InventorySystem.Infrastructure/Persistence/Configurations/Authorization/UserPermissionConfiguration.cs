using InventorySystem.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations.Authorization;

public sealed class UserPermissionConfiguration
    : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(
        EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("UserPermissions");

        builder.HasKey(x =>
            new
            {
                x.UserId,
                x.PermissionId
            });

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}