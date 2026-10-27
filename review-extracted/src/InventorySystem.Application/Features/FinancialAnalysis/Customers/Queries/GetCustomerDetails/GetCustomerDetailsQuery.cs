using InventorySystem.Application.DTOs.FinancialAnalysis.Customers;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Customers.Queries.GetCustomerDetails;

public sealed record GetCustomerDetailsQuery(
    int FinancialPeriodId,
    int BranchId)
    : IRequest<CustomerEmployeeDetailsDto?>;
