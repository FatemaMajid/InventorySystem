using InventorySystem.Application.Common.Authorization;
using InventorySystem.Application.Features.Inventory.Home.Queries.GetHomeDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Permission:" + PermissionCodes.DashboardView)]
public class HomeController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetHomeDashboardQuery(),
            cancellationToken);

        return Ok(result);
    }
}