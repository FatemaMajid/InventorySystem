using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Expenses;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Expenses.Queries.GetExpenseDetails;

public sealed class GetExpenseDetailsQueryHandler
    : IRequestHandler<
        GetExpenseDetailsQuery,
        BranchExpenseDetailsDto?>
{
    private const string ExpenseRootCode =
        "36";

    private readonly IApplicationDbContext _context;

    public GetExpenseDetailsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BranchExpenseDetailsDto?> Handle(
        GetExpenseDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var periodExists =
            await _context.FinancialPeriods
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id ==
                            request.FinancialPeriodId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (!periodExists)
            return null;

        var branch =
            await _context.Branches
                .AsNoTracking()
                .Where(
                    x =>
                        x.Id ==
                            request.BranchId &&
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.BranchCode != "0" &&
                        x.BranchCode != "001" &&
                        x.BranchCode != "002")
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            x.BranchCode,
                            x.BranchNameArabic
                        })
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (branch is null)
            return null;

        var upload =
            await _context.FinancialUploads
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.FileType ==
                            FinancialFileType.IncomeStatement &&
                        x.BranchId ==
                            request.BranchId &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid)
                .OrderByDescending(
                    x => x.Version)
                .ThenByDescending(
                    x => x.Id)
                .Select(
                    x =>
                        new
                        {
                            x.Id
                        })
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (upload is null)
            return null;

        var rows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialUploadId ==
                            upload.Id &&
                        x.AccountCode.StartsWith(
                            ExpenseRootCode))
                .OrderBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var parent =
            rows
                .Where(
                    x =>
                        x.AccountCode !=
                            ExpenseRootCode &&
                        x.AccountCode.StartsWith(
                            ExpenseRootCode,
                            StringComparison.Ordinal))
                .OrderBy(
                    x => x.AccountCode.Length)
                .ThenBy(
                    x => x.RowOrder)
                .FirstOrDefault();

        if (parent is null)
            return null;

        var detailRows =
            rows
                .Where(
                    x =>
                        x.AccountCode !=
                            parent.AccountCode &&
                        x.AccountCode.StartsWith(
                            parent.AccountCode,
                            StringComparison.Ordinal))
                .OrderBy(
                    x => x.RowOrder)
                .Select(
                    x =>
                        new ExpenseDetailRowDto
                        {
                            AccountCode =
                                x.AccountCode,

                            AccountName =
                                x.AccountName,

                            Debit =
                                x.Debit ?? 0,

                            Credit =
                                x.Credit ?? 0,

                            Balance =
                                x.Balance ?? 0,

                            RowOrder =
                                x.RowOrder
                        })
                .ToList();

        return new BranchExpenseDetailsDto
        {
            FinancialPeriodId =
                request.FinancialPeriodId,

            BranchId =
                branch.Id,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchNameArabic,

            ParentAccountCode =
                parent.AccountCode,

            ParentAccountName =
                parent.AccountName,

            TotalExpenses =
                parent.Balance ?? 0,

            DetailsCount =
                detailRows.Count,

            Details =
                detailRows
        };
    }
}
