using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialParsedData;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetTransferRecords;

public record GetTransferRecordsQuery(
    int UploadId)
    : IRequest<List<TransferRecordDto>?>;
