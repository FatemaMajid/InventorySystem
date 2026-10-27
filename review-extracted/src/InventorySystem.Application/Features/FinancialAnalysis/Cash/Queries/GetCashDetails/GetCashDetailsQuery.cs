using InventorySystem.Application.DTOs.FinancialAnalysis.Cash;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Cash.Queries.GetCashDetails;

public sealed record GetCashDetailsQuery(
    int FinancialPeriodId,
    int BranchId)
    : IRequest<BranchCashDetailsDto?>;
