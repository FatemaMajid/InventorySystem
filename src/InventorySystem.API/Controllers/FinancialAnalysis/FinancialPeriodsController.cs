using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.ApproveFinancialPeriod;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.CreateFinancialPeriod;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetAllFinancialPeriods;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/periods")]
[Authorize]
public class FinancialPeriodsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialPeriodsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(
            new GetAllFinancialPeriodsQuery());

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetFinancialPeriodByIdQuery
            {
                Id = id
            });

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:FinancialPeriod.Create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateFinancialPeriodDto dto)
    {
        var id = await _mediator.Send(
            new CreateFinancialPeriodCommand
            {
                FinancialPeriod = dto
            });

        return Created(
            $"api/financial-analysis/periods/{id}",
            id);
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Policy = "Permission:FinancialPeriod.Approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _mediator.Send(
            new ApproveFinancialPeriodCommand
            {
                Id = id
            });

        if (!result)
            return NotFound();

        return NoContent();
    }
}
