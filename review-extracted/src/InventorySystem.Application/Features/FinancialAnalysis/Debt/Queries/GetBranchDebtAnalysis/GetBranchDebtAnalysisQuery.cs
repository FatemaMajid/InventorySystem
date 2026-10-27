using InventorySystem.Application.DTOs.FinancialAnalysis.Debt;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Debt.Queries.GetBranchDebtAnalysis;

public sealed record GetBranchDebtAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<BranchDebtAnalysisDto?>;
