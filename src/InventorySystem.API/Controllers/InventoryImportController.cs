using InventorySystem.Application.Features.InventorySessions.Commands.ConfirmInventoryImport;
using InventorySystem.Application.Features.InventorySessions.Import.Preview;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/inventory-import")]
public class InventoryImportController : ControllerBase
{
    private readonly InventoryImportPreviewService _previewService;
    private readonly IMediator _mediator;

    public InventoryImportController(
        InventoryImportPreviewService previewService,
        IMediator mediator)
    {
        _previewService = previewService;
        _mediator = mediator;
    }

    // =========================================================
    // PREVIEW
    // =========================================================

    [HttpPost("preview")]
    [Consumes("multipart/form-data")]
    public ActionResult<InventoryImportPreviewResponse> Preview(
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "يرجى اختيار ملف Excel."
            });
        }

        var extension =
            Path.GetExtension(file.FileName);

        var allowedExtensions =
            new[] { ".xlsx", ".xlsm" };

        if (!allowedExtensions.Contains(
                extension,
                StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message =
                    "صيغة الملف غير مدعومة. " +
                    "يرجى رفع ملف Excel بصيغة .xlsx أو .xlsm."
            });
        }

        using var stream =
            file.OpenReadStream();

        var request =
            new InventoryImportPreviewRequest
            {
                FileStream = stream,
                FileName = file.FileName
            };

        var response =
            _previewService.BuildPreview(
                request);

        return Ok(response);
    }

    // =========================================================
    // CONFIRM
    // =========================================================

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(
        [FromBody] ConfirmInventoryImportCommand command,
        CancellationToken cancellationToken)
    {
        var sessionId =
            await _mediator.Send(
                command,
                cancellationToken);

        return Ok(new
        {
            message = "تم تأكيد الجرد وحفظه بنجاح.",
            sessionId
        });
    }
}