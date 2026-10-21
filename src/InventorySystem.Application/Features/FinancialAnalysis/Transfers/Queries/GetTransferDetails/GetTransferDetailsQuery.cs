using InventorySystem.Application.DTOs.FinancialAnalysis.Transfers;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Transfers.Queries.GetTransferDetails;

public sealed record GetTransferDetailsQuery(
    int FinancialPeriodId,
    int BranchId)
    : IRequest<BranchTransferDetailsDto?>;
