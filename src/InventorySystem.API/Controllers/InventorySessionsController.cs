using InventorySystem.Application.Features.InventorySessions.Import.Confirm;
using InventorySystem.Application.Features.InventorySessions.Import.Preview;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparison;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventorySessions;
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
        if (query.SessionId != id)
            return BadRequest("Session ID does not match.");

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
        IFormFile file,
        [FromQuery] InventoryType inventoryType)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Excel file is required.");

        await using var stream = file.OpenReadStream();

        var request = new InventoryImportConfirmRequest
        {
            FileStream = stream,
            FileName = file.FileName,
            InventoryType = inventoryType
        };

        return Ok(await _confirmService.ConfirmAsync(request));
    }
}