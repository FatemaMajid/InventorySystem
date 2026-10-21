using InventorySystem.Application.DTOs.FinancialAnalysis.Inventory;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.Inventory.Queries.GetInventoryAnalysis;

public sealed record GetInventoryAnalysisQuery(
    int FinancialPeriodId)
    : IRequest<InventoryAnalysisDto?>;
