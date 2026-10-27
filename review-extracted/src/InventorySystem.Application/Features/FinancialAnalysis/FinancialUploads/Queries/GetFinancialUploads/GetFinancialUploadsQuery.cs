using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialUpload;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetFinancialUploads;

public record GetFinancialUploadsQuery(
    int FinancialPeriodId)
    : IRequest<List<FinancialUploadDto>>;
