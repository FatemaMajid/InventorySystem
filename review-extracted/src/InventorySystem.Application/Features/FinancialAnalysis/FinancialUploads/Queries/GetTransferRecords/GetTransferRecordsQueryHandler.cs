using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialParsedData;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetTransferRecords;

public class GetTransferRecordsQueryHandler
    : IRequestHandler<
        GetTransferRecordsQuery,
        List<TransferRecordDto>?>
{
    private readonly IApplicationDbContext _context;

    public GetTransferRecordsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransferRecordDto>?> Handle(
        GetTransferRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var upload =
            await _context.FinancialUploads
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.UploadId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (upload is null)
            return null;

        if (upload.FileType !=
            FinancialFileType.Transfer)
        {
            throw new InvalidOperationException(
                "Transfer records are only available for transfer uploads.");
        }

        return await _context.TransferRecords
            .AsNoTracking()
            .Where(
                x =>
                    x.FinancialUploadId ==
                        request.UploadId)
            .OrderBy(x => x.RowOrder)
            .Select(
                x =>
                    new TransferRecordDto
                    {
                        Id = x.Id,
                        FinancialUploadId =
                            x.FinancialUploadId,
                        BranchId =
                            x.BranchId,
                        BranchCode =
                            x.Branch != null
                                ? x.Branch.BranchCode
                                : string.Empty,
                        BranchNameArabic =
                            x.Branch != null
                                ? x.Branch.BranchNameArabic
                                : string.Empty,
                        AccountCode =
                            x.AccountCode,
                        AccountName =
                            x.AccountName,
                        Gender = x.Gender,
                        Value = x.Value,
                        RowOrder = x.RowOrder
                    })
            .ToListAsync(
                cancellationToken);
    }
}
