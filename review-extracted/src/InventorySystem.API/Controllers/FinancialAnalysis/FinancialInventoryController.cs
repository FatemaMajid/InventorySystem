using InventorySystem.Application.Features.FinancialAnalysis.Inventory.Queries.GetInventoryAnalysis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/inventory")]
[Authorize]
public sealed class FinancialInventoryController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialInventoryController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialInventory.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetInventoryAnalysisQuery(
                    periodId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
