using InventorySystem.Application.Features.InventorySessions.Import.Confirm;
using InventorySystem.Application.Features.InventorySessions.Import.Preview;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparison;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventorySessions;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryDashboard;
using InventorySystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventorySessionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly InventoryImportPreviewService _previewService;
    private readonly InventoryImportConfirmService _confirmService;

    public InventorySessionsController(
        IMediator mediator,
        InventoryImportPreviewService previewService,
        InventoryImportConfirmService confirmService)
    {
        _mediator = mediator;
        _previewService = previewService;
        _confirmService = confirmService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetInventorySessionsQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("{id:int}/comparison")]
    public async Task<IActionResult> GetComparison(
    int id,
    [FromQuery] GetInventoryComparisonQuery query)
    {
        query = query with { SessionId = id };

        return Ok(await _mediator.Send(query));
    }

    [HttpPost("import/preview")]
    public async Task<IActionResult> Preview(
        IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Excel file is required.");

        await using var stream = file.OpenReadStream();

        var request = new InventoryImportPreviewRequest
        {
            FileStream = stream,
            FileName = file.FileName
        };

        return Ok(_previewService.BuildPreview(request));
    }

    [HttpPost("import/confirm")]
    public async Task<IActionResult> Confirm(
    IFormFile beforeFile,
    IFormFile afterFile,
    [FromQuery] InventoryType inventoryType)
    {
        if (beforeFile == null || beforeFile.Length == 0)
            return BadRequest("Before inventory Excel file is required.");

        if (afterFile == null || afterFile.Length == 0)
            return BadRequest("After inventory Excel file is required.");

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
    public async Task<IActionResult> GetDashboard(
    int id)
    {
        var query = new GetInventoryDashboardQuery(id);

        return Ok(await _mediator.Send(query));
    }
}