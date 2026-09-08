using MediatR;

namespace InventorySystem.Application.Features.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(int Id) : IRequest;