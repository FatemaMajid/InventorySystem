using InventorySystem.Application.DTOs.FinancialAnalysis.Expenses;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Expenses.Queries.GetExpenseDetails;

public sealed record GetExpenseDetailsQuery(
    int FinancialPeriodId,
    int BranchId)
    : IRequest<BranchExpenseDetailsDto?>;
