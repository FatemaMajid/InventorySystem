using InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Commands.UploadFinancialFile;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetFinancialUploads;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

[ApiController]
[Route("api/financial-analysis/uploads")]
[Authorize]
public class FinancialUploadsController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialUploadsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("period/{financialPeriodId:int}")]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.View")]
    public async Task<IActionResult> GetByPeriod(
        int financialPeriodId)
    {
        var result =
            await _mediator.Send(
                new GetFinancialUploadsQuery(
                    financialPeriodId));

        return Ok(result);
    }

    [HttpPost]
    [Authorize(
        Policy =
            "Permission:FinancialPeriod.Create")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
        [FromForm] FinancialUploadRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null ||
            request.File.Length == 0)
        {
            return BadRequest(
                "File is required.");
        }

        await using var memoryStream =
            new MemoryStream();

        await request.File.CopyToAsync(
            memoryStream,
            cancellationToken);

        var command =
            new UploadFinancialFileCommand
            {
                FinancialPeriodId =
                    request.FinancialPeriodId,

                FileType =
                    request.FileType,

                Scope =
                    request.Scope,

                BranchId =
                    request.BranchId,

                Gender =
                    request.Gender,

                OriginalFileName =
                    request.File.FileName,

                Content =
                    memoryStream.ToArray()
            };

        var result =
            await _mediator.Send(
                command,
                cancellationToken);

        return Created(
            $"api/financial-analysis/uploads/{result.Id}",
            result);
    }
}
