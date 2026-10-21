using InventorySystem.Application.DTOs.FinancialAnalysis.Capital;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Capital.Queries.GetCapitalAnalysis;

public sealed record GetCapitalAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<CapitalAnalysisDto?>;
