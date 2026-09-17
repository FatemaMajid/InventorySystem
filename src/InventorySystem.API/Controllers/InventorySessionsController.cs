using InventorySystem.API.Authorization;
using InventorySystem.Application.Common.Authorization;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.InventorySessions.Import.Confirm;
using InventorySystem.Application.Features.InventorySessions.Import.Preview;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparison;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryDashboard;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventorySessions;
using InventorySystem.Application.Features.InventorySessions.Queries.GetAttentionItems;
using InventorySystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventorySessionsController : ControllerBase
{
    private const long MaxExcelFileSize = 50 * 1024 * 1024;

    private readonly IMediator _mediator;
    private readonly InventoryImportPreviewService _previewService;
    private readonly InventoryImportConfirmService _confirmService;
    private readonly IDashboardExportService _dashboardExportService;
    private readonly IComparisonExportService _comparisonExportService;

    public InventorySessionsController(
        IMediator mediator,
        InventoryImportPreviewService previewService,
        InventoryImportConfirmService confirmService,
        IDashboardExportService dashboardExportService,
        IComparisonExportService comparisonExportService
        )
    {
        _mediator = mediator;
        _previewService = previewService;
        _confirmService = confirmService;
        _dashboardExportService = dashboardExportService;
        _comparisonExportService = comparisonExportService;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.InventorySessionView)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetInventorySessionsQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("{id:int}/attention")]
    [HasPermission(PermissionCodes.AttentionView)]
    public async Task<IActionResult> GetAttentionItems(
    int id,
    [FromQuery] GetAttentionItemsQuery query)
    {
        query = query with { SessionId = id };

        return Ok(await _mediator.Send(query));
    }

    [HttpGet("{id:int}/comparison")]
    [HasPermission(PermissionCodes.ComparisonView)]
    public async Task<IActionResult> GetComparison(
        int id,
        [FromQuery] GetInventoryComparisonQuery query)
    {
        query = query with { SessionId = id };
        return Ok(await _mediator.Send(query));
    }

    [HttpPost("import/preview")]
    [HasPermission(PermissionCodes.InventorySessionCreate)]
    public async Task<IActionResult> Preview(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Excel file is required.");

        if (file.Length > MaxExcelFileSize)
            return BadRequest("Excel file size cannot exceed 10 MB.");

        if (!string.Equals(
                Path.GetExtension(file.FileName),
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .xlsx Excel files are allowed.");

        await using var stream = file.OpenReadStream();

        var request = new InventoryImportPreviewRequest
        {
            FileStream = stream,
            FileName = file.FileName
        };

        return Ok(_previewService.BuildPreview(request));
    }

    [HttpPost("import/confirm")]
    [HasPermission(PermissionCodes.InventorySessionCreate)]
    public async Task<IActionResult> Confirm(
        IFormFile beforeFile,
        IFormFile afterFile,
        [FromQuery] InventoryType inventoryType)
    {
        if (beforeFile == null || beforeFile.Length == 0)
            return BadRequest("Before inventory Excel file is required.");

        if (afterFile == null || afterFile.Length == 0)
            return BadRequest("After inventory Excel file is required.");

        if (beforeFile.Length > MaxExcelFileSize)
            return BadRequest("Before inventory Excel file size cannot exceed 10 MB.");

        if (afterFile.Length > MaxExcelFileSize)
            return BadRequest("After inventory Excel file size cannot exceed 10 MB.");

        if (!string.Equals(
                Path.GetExtension(beforeFile.FileName),
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
            return BadRequest("Before inventory file must be an .xlsx Excel file.");

        if (!string.Equals(
                Path.GetExtension(afterFile.FileName),
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
            return BadRequest("After inventory file must be an .xlsx Excel file.");

        await using var beforeStream = beforeFile.OpenReadStream();
        await using var afterStream = afterFile.OpenReadStream();

        var request = new InventoryImportConfirmRequest
        {
            BeforeFileStream = beforeStream,
            BeforeFileName = beforeFile.FileName,
            AfterFileStream = afterStream,
            AfterFileName = afterFile.FileName,
            InventoryType = inventoryType
        };

        return Ok(await _confirmService.ConfirmAsync(request));
    }

    [HttpGet("{id:int}/dashboard")]
    [HasPermission(PermissionCodes.DashboardView)]
    public async Task<IActionResult> GetDashboard(int id)
    {
        var query = new GetInventoryDashboardQuery(id);
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("{id:int}/dashboard/export/excel")]
    [HasPermission(PermissionCodes.DashboardExport)]
    public async Task<IActionResult> ExportDashboardExcel(
        int id,
        [FromQuery] string language = "ar",
        CancellationToken cancellationToken = default)
    {
        language = language.ToLowerInvariant();

        if (language != "ar" && language != "en")
            language = "ar";

        var file = await _dashboardExportService.ExportExcelAsync(
            id,
            language,
            cancellationToken);

        var fileName = language == "ar"
            ? $"تقرير_الجرد_{id}.xlsx"
            : $"Inventory_Report_{id}.xlsx";

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet("{id:int}/dashboard/export/pdf")]
    [HasPermission(PermissionCodes.DashboardExport)]
    public async Task<IActionResult> ExportDashboardPdf(
        int id,
        [FromQuery] string language = "ar",
        CancellationToken cancellationToken = default)
    {
        language = language.ToLowerInvariant();

        if (language != "ar" && language != "en")
            language = "ar";

        var file = await _dashboardExportService.ExportPdfAsync(
            id,
            language,
            cancellationToken);

        var fileName = language == "ar"
            ? $"تقرير_الجرد_{id}.pdf"
            : $"Inventory_Report_{id}.pdf";

        return File(file, "application/pdf", fileName);
    }

    // ==========================================
    // Export Comparison Excel

    [HttpGet("{id:int}/comparison/export/excel")]
    [HasPermission(PermissionCodes.ComparisonExport)]
    public async Task<IActionResult> ExportComparisonExcel(
    int id,
    [FromQuery] string? itemCode,
    [FromQuery] string? itemName,
    [FromQuery] int? categoryId,
    [FromQuery] int? unitId,
    [FromQuery] string? status,
    [FromQuery] string sortBy = "ItemCode",
    [FromQuery] bool descending = false,
    [FromQuery] string language = "ar",
    CancellationToken cancellationToken = default)
    {
        language = language == "en" ? "en" : "ar";

        var file = await _comparisonExportService.ExportExcelAsync(
            id, itemCode, itemName, categoryId, unitId, status,
            sortBy, descending, language, cancellationToken);

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            language == "ar"
                ? $"مقارنة_الجرد_{id}.xlsx"
                : $"Inventory_Comparison_{id}.xlsx");
    }

    // ==========================================
    // Export Comparison PDF

    [HttpGet("{id:int}/comparison/export/pdf")]
    [HasPermission(PermissionCodes.ComparisonExport)]
    public async Task<IActionResult> ExportComparisonPdf(
        int id,
        [FromQuery] string? itemCode,
        [FromQuery] string? itemName,
        [FromQuery] int? categoryId,
        [FromQuery] int? unitId,
        [FromQuery] string? status,
        [FromQuery] string sortBy = "ItemCode",
        [FromQuery] bool descending = false,
        [FromQuery] string language = "ar",
        CancellationToken cancellationToken = default)
    {
        language = language == "en" ? "en" : "ar";

        var file = await _comparisonExportService.ExportPdfAsync(
            id, itemCode, itemName, categoryId, unitId, status,
            sortBy, descending, language, cancellationToken);

        return File(
            file,
            "application/pdf",
            language == "ar"
                ? $"مقارنة_الجرد_{id}.pdf"
                : $"Inventory_Comparison_{id}.pdf");
    }
}