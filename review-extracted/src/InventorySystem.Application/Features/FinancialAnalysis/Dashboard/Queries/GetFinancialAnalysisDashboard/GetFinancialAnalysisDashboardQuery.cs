using InventorySystem.Application.DTOs.FinancialAnalysis.DashboardAggregate;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Dashboard.Queries.GetFinancialAnalysisDashboard;

public sealed record GetFinancialAnalysisDashboardQuery(
    int FinancialPeriodId,
    int? BranchId,
    IReadOnlyCollection<string> Sections,
    bool IncludeDetails = false)
    : IRequest<FinancialAnalysisDashboardDto?>;
