using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialParsedData;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetFinancialAccountRows;

public record GetFinancialAccountRowsQuery(
    int UploadId)
    : IRequest<List<FinancialAccountRowDto>?>;
