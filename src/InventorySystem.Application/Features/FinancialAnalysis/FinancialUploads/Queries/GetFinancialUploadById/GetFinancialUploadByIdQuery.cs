using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialUpload;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetFinancialUploadById;

public record GetFinancialUploadByIdQuery(
    int UploadId)
    : IRequest<FinancialUploadDto?>;
