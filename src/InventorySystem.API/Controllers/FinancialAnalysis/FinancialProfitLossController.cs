using InventorySystem.Application.Features.FinancialAnalysis.ProfitLoss.Queries.GetProfitLossAnalysis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/profit-loss")]
[Authorize]
public sealed class FinancialProfitLossController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialProfitLossController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialProfitLoss.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetProfitLossAnalysisQuery(
                    periodId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
