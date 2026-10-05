using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetAllFinancialPeriods;

public record GetAllFinancialPeriodsQuery
    : IRequest<List<FinancialPeriodDto>>;
