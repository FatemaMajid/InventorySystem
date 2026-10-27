using InventorySystem.Application.DTOs.FinancialAnalysis.Expenses;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Expenses.Queries.GetExpensesAnalysis;

public sealed record GetExpensesAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<ExpensesAnalysisDto?>;
