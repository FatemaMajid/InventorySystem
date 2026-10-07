using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialMonitoring;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodSummary;

public record GetFinancialPeriodSummaryQuery(int FinancialPeriodId)
    : IRequest<FinancialPeriodSummaryDto?>;
