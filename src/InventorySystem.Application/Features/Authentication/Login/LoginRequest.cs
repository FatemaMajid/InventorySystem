using MediatR;

namespace InventorySystem.Application.Features.Authentication.Login;

public sealed record LoginRequest(
    string Username,
    string Password)
    : IRequest<LoginResponse>;