using InventorySystem.Application.DTOs.FinancialAnalysis.Customers;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Customers.Queries.GetCustomersAnalysis;

public sealed record GetCustomersAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<CustomersAnalysisDto?>;
