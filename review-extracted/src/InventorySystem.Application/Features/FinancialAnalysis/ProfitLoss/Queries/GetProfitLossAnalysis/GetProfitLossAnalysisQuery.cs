using InventorySystem.Application.DTOs.FinancialAnalysis.ProfitLoss;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.ProfitLoss.Queries.GetProfitLossAnalysis;

public sealed record GetProfitLossAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<ProfitLossAnalysisDto?>;
