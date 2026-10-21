using InventorySystem.Application.Features.FinancialAnalysis.Expenses.Queries.GetExpenseDetails;
using InventorySystem.Application.Features.FinancialAnalysis.Expenses.Queries.GetExpensesAnalysis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/expenses")]
[Authorize]
public sealed class FinancialExpensesController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialExpensesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialExpenses.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetExpensesAnalysisQuery(
                    periodId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{branchId:int}/details")]
    [Authorize(
        Policy =
            "Permission:FinancialExpenses.View")]
    public async Task<IActionResult> GetDetails(
        int branchId,
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetExpenseDetailsQuery(
                    periodId,
                    branchId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
