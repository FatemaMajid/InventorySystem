using InventorySystem.Application.Features.FinancialAnalysis.Cash.Queries.GetCashAnalysis;
using InventorySystem.Application.Features.FinancialAnalysis.Cash.Queries.GetCashDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/cash")]
[Authorize]
public sealed class FinancialCashController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialCashController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialCash.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetCashAnalysisQuery(
                    periodId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{branchId:int}/details")]
    [Authorize(
        Policy =
            "Permission:FinancialCash.View")]
    public async Task<IActionResult> GetDetails(
        int branchId,
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetCashDetailsQuery(
                    periodId,
                    branchId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
