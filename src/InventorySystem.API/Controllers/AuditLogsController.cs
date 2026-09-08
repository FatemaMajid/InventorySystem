using InventorySystem.Application.Common.Authorization;
using InventorySystem.Application.Features.AuditLogs.Queries.GetAuditLogById;
using InventorySystem.Application.Features.AuditLogs.Queries.GetAuditLogs;
using InventorySystem.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AuditLogView)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAuditLogsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.AuditLogView)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAuditLogByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }
}