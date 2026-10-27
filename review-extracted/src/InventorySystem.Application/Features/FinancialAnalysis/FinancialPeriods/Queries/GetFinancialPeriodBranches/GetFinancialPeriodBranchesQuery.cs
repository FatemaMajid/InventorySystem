using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialMonitoring;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodBranches;

public record GetFinancialPeriodBranchesQuery(int FinancialPeriodId)
    : IRequest<List<FinancialBranchStatusDto>?>;
