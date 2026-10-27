using InventorySystem.Application.DTOs.FinancialAnalysis.Cash;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Cash.Queries.GetCashAnalysis;

public sealed record GetCashAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<CashAnalysisDto?>;
