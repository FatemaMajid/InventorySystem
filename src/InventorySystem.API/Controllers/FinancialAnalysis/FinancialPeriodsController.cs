using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.ApproveFinancialPeriod;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.CreateFinancialPeriod;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.DeleteFinancialPeriod;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.UpdateFinancialPeriod;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetAllFinancialPeriods;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialDashboard;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodBranches;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodById;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodSummary;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetMissingFinancialFiles;
using InventorySystem.Application.Features.FinancialAnalysis.Interfaces;
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
    private readonly IFinancialPeriodExportService _financialPeriodExportService;

    public FinancialPeriodsController(
        IMediator mediator,
        IFinancialPeriodExportService financialPeriodExportService)
    {
        _mediator = mediator;
        _financialPeriodExportService =
            financialPeriodExportService;
    }

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetAll()
    {
        var result =
            await _mediator.Send(
                new GetAllFinancialPeriodsQuery());

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var result =
            await _mediator.Send(
                new GetFinancialPeriodByIdQuery
                {
                    Id = id
                });

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id:int}/summary")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetSummary(
        int id)
    {
        var result =
            await _mediator.Send(
                new GetFinancialPeriodSummaryQuery(
                    id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id:int}/dashboard")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetDashboard(
        int id)
    {
        var result =
            await _mediator.Send(
                new GetFinancialDashboardQuery(
                    id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id:int}/missing-files")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetMissingFiles(
        int id)
    {
        var result =
            await _mediator.Send(
                new GetMissingFinancialFilesQuery(
                    id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id:int}/branches")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetBranches(
        int id)
    {
        var result =
            await _mediator.Send(
                new GetFinancialPeriodBranchesQuery(
                    id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.Create")]
    public async Task<IActionResult> Create(
        [FromBody]
        CreateFinancialPeriodDto dto)
    {
        var id =
            await _mediator.Send(
                new CreateFinancialPeriodCommand
                {
                    FinancialPeriod = dto
                });

        return Created(
            $"api/financial-analysis/periods/{id}",
            id);
    }

    [HttpPut("{id:int}")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.Create")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody]
        UpdateFinancialPeriodDto dto)
    {
        var result =
            await _mediator.Send(
                new UpdateFinancialPeriodCommand
                {
                    Id = id,
                    FinancialPeriod = dto
                });

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.Create")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var result =
            await _mediator.Send(
                new DeleteFinancialPeriodCommand
                {
                    Id = id
                });

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.Approve")]
    public async Task<IActionResult> Approve(
        int id)
    {
        var result =
            await _mediator.Send(
                new ApproveFinancialPeriodCommand
                {
                    Id = id
                });

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id:int}/export/excel")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.View")]
    [Authorize(
        Policy =
            "Permission:Report.Export")]
    public async Task<IActionResult> ExportExcel(
        int id,
        [FromQuery]
        string language = "ar",
        [FromQuery]
        string mode = "all",
        [FromQuery]
        int? branchId = null,
        CancellationToken cancellationToken = default)
    {
        language =
            language.ToLowerInvariant();

        if (language != "ar" &&
            language != "en")
        {
            language = "ar";
        }

        mode =
            string.IsNullOrWhiteSpace(mode)
                ? "all"
                : mode.ToLowerInvariant();

        var file =
            await _financialPeriodExportService
                .ExportExcelAsync(
                    id,
                    language,
                    mode,
                    branchId,
                    cancellationToken);

        var scope =
            mode == "branch"
                ? $"Branch_{branchId}"
                : "All_Branches";

        var fileName =
            $"Financial_Period_{id}_{scope}_{language.ToUpperInvariant()}.xlsx";

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
