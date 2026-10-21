using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialParsedData;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetFinancialAccountRows;

public class GetFinancialAccountRowsQueryHandler
    : IRequestHandler<
        GetFinancialAccountRowsQuery,
        List<FinancialAccountRowDto>?>
{
    private readonly IApplicationDbContext _context;

    public GetFinancialAccountRowsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FinancialAccountRowDto>?> Handle(
        GetFinancialAccountRowsQuery request,
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
                FinancialFileType.IncomeStatement &&
            upload.FileType !=
                FinancialFileType.FinancialPosition)
        {
            throw new InvalidOperationException(
                "Account rows are only available for income statement and financial position uploads.");
        }

        return await _context.FinancialAccountRows
            .AsNoTracking()
            .Where(
                x =>
                    x.FinancialUploadId ==
                        request.UploadId)
            .OrderBy(x => x.RowOrder)
            .Select(
                x =>
                    new FinancialAccountRowDto
                    {
                        Id = x.Id,
                        FinancialUploadId =
                            x.FinancialUploadId,
                        AccountCode =
                            x.AccountCode,
                        AccountName =
                            x.AccountName,
                        Debit = x.Debit,
                        Credit = x.Credit,
                        Balance = x.Balance,
                        Assets = x.Assets,
                        Liabilities =
                            x.Liabilities,
                        RowOrder = x.RowOrder
                    })
            .ToListAsync(
                cancellationToken);
    }
}
