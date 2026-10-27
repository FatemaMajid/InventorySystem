using InventorySystem.Application.Features.FinancialAnalysis.Transfers.Queries.GetTransferDetails;
using InventorySystem.Application.Features.FinancialAnalysis.Transfers.Queries.GetTransfersAnalysis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/transfers")]
[Authorize]
public sealed class FinancialTransfersController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialTransfersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialTransfers.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetTransfersAnalysisQuery(
                    periodId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{branchId:int}/details")]
    [Authorize(
        Policy =
            "Permission:FinancialTransfers.View")]
    public async Task<IActionResult> GetDetails(
        int branchId,
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetTransferDetailsQuery(
                    periodId,
                    branchId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
