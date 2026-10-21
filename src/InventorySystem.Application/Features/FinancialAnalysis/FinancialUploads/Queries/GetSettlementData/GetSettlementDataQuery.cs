using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialParsedData;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetSettlementData;

public record GetSettlementDataQuery(
    int UploadId)
    : IRequest<SettlementDataDto?>;
