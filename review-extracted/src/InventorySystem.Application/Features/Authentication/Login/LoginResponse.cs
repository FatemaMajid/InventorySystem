namespace InventorySystem.Application.Features.Authentication.Login;

public sealed record LoginResponse(
    int UserId,
    string Username,
    string Role,
    string Token,
    DateTime ExpiresAt
);