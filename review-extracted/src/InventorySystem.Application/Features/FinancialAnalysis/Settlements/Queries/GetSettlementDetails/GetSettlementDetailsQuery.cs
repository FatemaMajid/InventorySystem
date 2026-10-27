using InventorySystem.Application.DTOs.FinancialAnalysis.Settlements;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Settlements.Queries.GetSettlementDetails;

public sealed record GetSettlementDetailsQuery(
    int FinancialPeriodId,
    int BranchId)
    : IRequest<BranchSettlementDetailsDto?>;
