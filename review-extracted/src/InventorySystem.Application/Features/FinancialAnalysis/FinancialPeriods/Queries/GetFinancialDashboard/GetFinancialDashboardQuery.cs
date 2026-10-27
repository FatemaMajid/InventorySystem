using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialDashboard;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialDashboard;

public record GetFinancialDashboardQuery(
    int FinancialPeriodId)
    : IRequest<FinancialDashboardDto?>;
