using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialParsedData;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetSettlementData;

public class GetSettlementDataQueryHandler
    : IRequestHandler<
        GetSettlementDataQuery,
        SettlementDataDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSettlementDataQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SettlementDataDto?> Handle(
        GetSettlementDataQuery request,
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
            FinancialFileType.Settlement)
        {
            throw new InvalidOperationException(
                "Settlement data is only available for settlement uploads.");
        }

        var rows =
            await _context.SettlementRows
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialUploadId ==
                            request.UploadId)
                .OrderBy(x => x.RowOrder)
                .Select(
                    x =>
                        new SettlementRowDto
                        {
                            Id = x.Id,
                            FinancialUploadId =
                                x.FinancialUploadId,
                            TransactionDate =
                                x.TransactionDate,
                            Description =
                                x.Description,
                            EntryNumber =
                                x.EntryNumber,
                            Debit = x.Debit,
                            Credit = x.Credit,
                            Balance = x.Balance,
                            IsMarked =
                                x.IsMarked,
                            IsMatched =
                                x.IsMatched,
                            RowOrder =
                                x.RowOrder
                        })
                .ToListAsync(
                    cancellationToken);

        var summary =
            await _context.SettlementSummaries
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialUploadId ==
                            request.UploadId)
                .Select(
                    x =>
                        new SettlementSummaryDto
                        {
                            Id = x.Id,
                            FinancialUploadId =
                                x.FinancialUploadId,
                            PeriodDebit =
                                x.PeriodDebit,
                            PeriodCredit =
                                x.PeriodCredit,
                            PeriodBalance =
                                x.PeriodBalance
                        })
                .FirstOrDefaultAsync(
                    cancellationToken);

        return new SettlementDataDto
        {
            FinancialUploadId =
                request.UploadId,
            Rows = rows,
            Summary = summary
        };
    }
}
