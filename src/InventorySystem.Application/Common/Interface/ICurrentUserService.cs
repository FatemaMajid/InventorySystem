namespace InventorySystem.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }

    string? Username { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsManager { get; }

    bool IsAdmin { get; }
}