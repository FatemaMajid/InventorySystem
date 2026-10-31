using InventorySystem.Application.Features.FinancialAnalysis.Dashboard.Interfaces;
using InventorySystem.Application.Features.FinancialAnalysis.Dashboard.Queries.GetFinancialAnalysisDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/dashboard")]
[Authorize]
public sealed class FinancialAnalysisDashboardController
    : ControllerBase
{
    private readonly IMediator _mediator;

    private readonly
        IFinancialDashboardExcelExportService
        _excelExportService;

    private readonly
        IFinancialDashboardPdfExportService
        _pdfExportService;

    public FinancialAnalysisDashboardController(
        IMediator mediator,
        IFinancialDashboardExcelExportService excelExportService,
        IFinancialDashboardPdfExportService pdfExportService)
    {
        _mediator =
            mediator;

        _excelExportService =
            excelExportService;

        _pdfExportService =
            pdfExportService;
    }

    // ========================================================
    // DASHBOARD DATA
    // ========================================================

    [HttpGet]
    [Authorize(
        Policy =
            "Permission:FinancialDashboard.View")]
    public async Task<IActionResult> Get(
        [FromQuery]
        int periodId,

        [FromQuery]
        string branchId = "all",

        [FromQuery]
        string sections = "all",

        CancellationToken cancellationToken = default)
    {
        var parsed =
            ParseRequest(
                branchId,
                sections);

        var result =
            await _mediator.Send(
                new GetFinancialAnalysisDashboardQuery(
                    periodId,
                    parsed.BranchId,
                    parsed.Sections),
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(
            result);
    }

    // ========================================================
    // EXCEL EXPORT
    // ========================================================

    [HttpGet("export/excel")]
    [Authorize(
        Policy =
            "Permission:FinancialDashboard.View")]
    [Authorize(
        Policy =
            "Permission:Report.Export")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery]
        int periodId,

        [FromQuery]
        string branchId = "all",

        [FromQuery]
        string sections = "all",

        [FromQuery]
        string language = "ar",

        CancellationToken cancellationToken = default)
    {
        language =
            string.Equals(
                language,
                "en",
                StringComparison.OrdinalIgnoreCase)
                ? "en"
                : "ar";

        var parsed =
            ParseRequest(
                branchId,
                sections);

        var dashboard =
            await _mediator.Send(
                new GetFinancialAnalysisDashboardQuery(
                    periodId,
                    parsed.BranchId,
                    parsed.Sections,
                    true),
                cancellationToken);

        if (dashboard is null)
            return NotFound();

        var bytes =
            await _excelExportService.ExportAsync(
                dashboard,
                language,
                cancellationToken);

        var branchPart =
            dashboard.IsAllBranches
                ? "All"
                : $"Branch_{dashboard.SelectedBranch?.BranchCode}";

        var languagePart =
            language.ToUpperInvariant();

        var fileName =
            $"Financial_Dashboard_Period_{periodId}_{branchPart}_{languagePart}.xlsx";

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }


    // ========================================================
    // PDF EXPORT
    // ========================================================

    [HttpGet("export/pdf")]
    [Authorize(
        Policy =
            "Permission:FinancialDashboard.View")]
    [Authorize(
        Policy =
            "Permission:Report.Export")]
    public async Task<IActionResult> ExportPdf(
        [FromQuery]
        int periodId,

        [FromQuery]
        string branchId = "all",

        [FromQuery]
        string sections = "all",

        [FromQuery]
        string language = "ar",

        CancellationToken cancellationToken = default)
    {
        language =
            string.Equals(
                language,
                "en",
                StringComparison.OrdinalIgnoreCase)
                ? "en"
                : "ar";

        var parsed =
            ParseRequest(
                branchId,
                sections);

        var dashboard =
            await _mediator.Send(
                new GetFinancialAnalysisDashboardQuery(
                    periodId,
                    parsed.BranchId,
                    parsed.Sections,
                    true),
                cancellationToken);

        if(dashboard is null)
            return NotFound();

        var bytes =
            await _pdfExportService.ExportAsync(
                dashboard,
                language,
                cancellationToken);

        var branchPart =
            dashboard.IsAllBranches
                ? "All"
                : $"Branch_{dashboard.SelectedBranch?.BranchCode}";

        var fileName =
            $"Financial_Dashboard_Period_{periodId}_{branchPart}_{language.ToUpperInvariant()}.pdf";

        return File(
            bytes,
            "application/pdf",
            fileName);
    }
    // ========================================================
    // REQUEST PARSER
    // ========================================================

    private static DashboardRequest ParseRequest(
        string branchId,
        string sections)
    {
        int? selectedBranchId =
            null;

        if (
            !string.IsNullOrWhiteSpace(
                branchId) &&
            !string.Equals(
                branchId,
                "all",
                StringComparison.OrdinalIgnoreCase)
        )
        {
            if (
                !int.TryParse(
                    branchId,
                    out var parsedBranchId) ||
                parsedBranchId <= 0
            )
            {
                throw new InvalidOperationException(
                    "FinancialAnalysis.InvalidBranchId");
            }

            selectedBranchId =
                parsedBranchId;
        }

        var requestedSections =
            string.IsNullOrWhiteSpace(
                sections)
                ? Array.Empty<string>()
                : sections.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

        return new DashboardRequest(
            selectedBranchId,
            requestedSections);
    }

    private sealed record DashboardRequest(
        int? BranchId,
        IReadOnlyCollection<string> Sections);
}
