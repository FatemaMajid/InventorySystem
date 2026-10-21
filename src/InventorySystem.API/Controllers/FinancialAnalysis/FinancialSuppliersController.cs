using InventorySystem.Application.Features.FinancialAnalysis.Suppliers.Queries.GetSupplierDetails;
using InventorySystem.Application.Features.FinancialAnalysis.Suppliers.Queries.GetSuppliersAnalysis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/suppliers")]
[Authorize]
public sealed class FinancialSuppliersController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialSuppliersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialSuppliers.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetSuppliersAnalysisQuery(
                    periodId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{branchId:int}/details")]
    [Authorize(
        Policy =
            "Permission:FinancialSuppliers.View")]
    public async Task<IActionResult> GetDetails(
        int branchId,
        [FromQuery]
        int periodId,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new GetSupplierDetailsQuery(
                    periodId,
                    branchId),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
