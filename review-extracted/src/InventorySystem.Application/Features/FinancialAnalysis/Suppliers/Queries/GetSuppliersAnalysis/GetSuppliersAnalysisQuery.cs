using InventorySystem.Application.DTOs.FinancialAnalysis.Suppliers;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Suppliers.Queries.GetSuppliersAnalysis;

public sealed record GetSuppliersAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<SuppliersAnalysisDto?>;
