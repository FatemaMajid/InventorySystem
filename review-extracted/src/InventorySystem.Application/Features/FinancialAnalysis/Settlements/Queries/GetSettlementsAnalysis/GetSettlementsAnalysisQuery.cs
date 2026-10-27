using InventorySystem.Application.DTOs.FinancialAnalysis.Settlements;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Settlements.Queries.GetSettlementsAnalysis;

public sealed record GetSettlementsAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<SettlementsAnalysisDto?>;
