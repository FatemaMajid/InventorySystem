using InventorySystem.Application.DTOs.FinancialAnalysis.Transfers;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Transfers.Queries.GetTransfersAnalysis;

public sealed record GetTransfersAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<TransfersAnalysisDto?>;
