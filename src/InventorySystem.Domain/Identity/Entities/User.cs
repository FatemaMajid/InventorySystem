namespace InventorySystem.Domain.Entities.Authorization;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Increment to revoke all previously issued JWTs for this user.
    public int TokenVersion { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();

    public ICollection<UserPermission> UserPermissions { get; set; }
        = new List<UserPermission>();
}