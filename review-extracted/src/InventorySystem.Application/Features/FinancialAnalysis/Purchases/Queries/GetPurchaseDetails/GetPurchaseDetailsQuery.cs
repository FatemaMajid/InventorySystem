using InventorySystem.Application.DTOs.FinancialAnalysis.Purchases;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Purchases.Queries.GetPurchaseDetails;

public sealed record GetPurchaseDetailsQuery(
    int FinancialPeriodId,
    int BranchId)
    : IRequest<BranchPurchaseDetailsDto?>;
