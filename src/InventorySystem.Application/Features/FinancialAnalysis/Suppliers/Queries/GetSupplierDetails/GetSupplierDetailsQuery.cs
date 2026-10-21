using InventorySystem.Application.DTOs.FinancialAnalysis.Suppliers;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Suppliers.Queries.GetSupplierDetails;

public sealed record GetSupplierDetailsQuery(
    int FinancialPeriodId,
    int BranchId)
    : IRequest<BranchSupplierDetailsDto?>;
