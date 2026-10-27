using InventorySystem.Application.Features.FinancialAnalysis.Debt.Queries.GetBranchDebtAnalysis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/debt")]
[Authorize]
public sealed class FinancialDebtController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialDebtController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialDebt.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetBranchDebtAnalysisQuery(
                    periodId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
