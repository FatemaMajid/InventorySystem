using InventorySystem.Application.Features.FinancialAnalysis.Settlements.Queries.GetSettlementDetails;
using InventorySystem.Application.Features.FinancialAnalysis.Settlements.Queries.GetSettlementsAnalysis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/settlements")]
[Authorize]
public sealed class FinancialSettlementsController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialSettlementsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialSettlements.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetSettlementsAnalysisQuery(
                    periodId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{branchId:int}/details")]
    [Authorize(
        Policy =
            "Permission:FinancialSettlements.View")]
    public async Task<IActionResult> GetDetails(
        int branchId,
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetSettlementDetailsQuery(
                    periodId,
                    branchId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
