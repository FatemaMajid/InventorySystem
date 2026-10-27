using InventorySystem.Application.DTOs.FinancialAnalysis.Sales;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Sales.Queries.GetSalesAnalysis;

public sealed record GetSalesAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<SalesAnalysisDto?>;
