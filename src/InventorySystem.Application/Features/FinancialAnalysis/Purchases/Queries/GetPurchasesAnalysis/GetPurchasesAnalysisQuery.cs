using InventorySystem.Application.DTOs.FinancialAnalysis.Purchases;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Purchases.Queries.GetPurchasesAnalysis;

public sealed record GetPurchasesAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<PurchasesAnalysisDto?>;
