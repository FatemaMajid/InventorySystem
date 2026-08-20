using InventorySystem.API.Authorization;
using InventorySystem.Application.Common.Authorization;
using InventorySystem.Application.Features.Users.Commands.CreateUser;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.UserCreate)]
    public async Task<ActionResult<CreateUserResponse>> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return Ok(result);
    }
}