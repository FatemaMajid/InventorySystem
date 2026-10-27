using InventorySystem.API.Authorization;
using InventorySystem.Application.Common.Authorization;
using InventorySystem.Application.Features.Users.Commands.CreateUser;
using InventorySystem.Application.Features.Users.Commands.DeleteUser;
using InventorySystem.Application.Features.Users.Commands.UpdateUser;
using InventorySystem.Application.Features.Users.Queries.GetAllUsers;
using InventorySystem.Application.Features.Users.Queries.GetUserById;
using InventorySystem.Application.Features.Users.Queries.GetUserOptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.UserView)]
    public async Task<ActionResult<IReadOnlyCollection<UserListItem>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(
            new GetAllUsersQuery(),
            cancellationToken));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.UserView)]
    public async Task<ActionResult<UserDetails>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUserByIdQuery(id),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("options")]
    [HasPermission(PermissionCodes.UserView)]
    public async Task<ActionResult<UserOptionsResponse>> GetOptions(
        CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(
            new GetUserOptionsQuery(),
            cancellationToken));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.UserCreate)]
    public async Task<ActionResult<CreateUserResponse>> Create(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(
            command,
            cancellationToken));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.UserEdit)]
    public async Task<ActionResult<UpdateUserResponse>> Update(
        int id,
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match request id.");
        }

        return Ok(await _sender.Send(
            command,
            cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [HasPermission(PermissionCodes.UserDeactivate)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteUserCommand(id),
            cancellationToken);

        return NoContent();
    }
}