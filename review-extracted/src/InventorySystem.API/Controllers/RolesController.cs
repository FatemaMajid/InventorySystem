using InventorySystem.API.Authorization;
using InventorySystem.Application.Common.Authorization;
using InventorySystem.Application.Features.Roles.Commands.CreateRole;
using InventorySystem.Application.Features.Roles.Commands.UpdateRole;
using InventorySystem.Application.Features.Roles.Queries.GetAllPermissions;
using InventorySystem.Application.Features.Roles.Queries.GetAllRoles;
using InventorySystem.Application.Features.Roles.Queries.GetRoleById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.RoleView)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllRolesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.RoleView)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetRoleByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("permissions")]
    [HasPermission(PermissionCodes.RoleView)]
    public async Task<IActionResult> GetPermissions(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllPermissionsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [HasPermission(PermissionCodes.RoleEdit)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.RoleEdit)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateRoleCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }
}