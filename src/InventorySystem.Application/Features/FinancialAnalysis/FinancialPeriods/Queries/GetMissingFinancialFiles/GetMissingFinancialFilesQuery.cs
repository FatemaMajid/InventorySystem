using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialMonitoring;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetMissingFinancialFiles;

public record GetMissingFinancialFilesQuery(int FinancialPeriodId)
    : IRequest<List<MissingFinancialFileDto>?>;
