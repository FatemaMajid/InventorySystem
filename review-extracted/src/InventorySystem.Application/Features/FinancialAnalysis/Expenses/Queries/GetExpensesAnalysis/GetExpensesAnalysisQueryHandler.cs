using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Expenses;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Expenses.Queries.GetExpensesAnalysis;

public sealed class GetExpensesAnalysisQueryHandler
    : IRequestHandler<
        GetExpensesAnalysisQuery,
        ExpensesAnalysisDto?>
{
    private const string ExpenseRootCode =
        "36";

    private readonly IApplicationDbContext _context;

    public GetExpensesAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExpensesAnalysisDto?> Handle(
        GetExpensesAnalysisQuery request,
        CancellationToken cancellationToken)
    {
        var period =
            await _context.FinancialPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                            request.FinancialPeriodId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (period is null)
            return null;

        var branches =
            await _context.Branches
                .AsNoTracking()
                .Where(
                    x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.BranchCode != "0" &&
                        x.BranchCode != "001" &&
                        x.BranchCode != "002")
                .OrderBy(
                    x => x.BranchCode)
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            x.BranchCode,
                            x.BranchNameArabic
                        })
                .ToListAsync(
                    cancellationToken);

        var branchIds =
            branches
                .Select(x => x.Id)
                .ToList();

        var uploads =
            await _context.FinancialUploads
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.FileType ==
                            FinancialFileType.IncomeStatement &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid &&
                        x.BranchId.HasValue &&
                        branchIds.Contains(
                            x.BranchId.Value))
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            BranchId =
                                x.BranchId!.Value,
                            x.Version
                        })
                .ToListAsync(
                    cancellationToken);

        var activeUploadsByBranch =
            uploads
                .GroupBy(x => x.BranchId)
                .ToDictionary(
                    x => x.Key,
                    x =>
                        x.OrderByDescending(y => y.Version)
                            .ThenByDescending(y => y.Id)
                            .First());

        var uploadIds =
            activeUploadsByBranch
                .Values
                .Select(x => x.Id)
                .ToList();

        /*
         * Expenses hierarchy:
         *
         * 36       = Root
         * 36002    = Branch Parent
         * 36002001 = Detail
         * 36002002 = Detail
         * ...
         *
         * We load the whole 36 hierarchy.
         */
        var expenseRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.AccountCode.StartsWith(
                            ExpenseRootCode))
                .OrderBy(x => x.FinancialUploadId)
                .ThenBy(x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var rowsByUpload =
            expenseRows
                .GroupBy(x => x.FinancialUploadId)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());

        var results =
            new List<BranchExpenseDto>();

        foreach (var branch in branches)
        {
            if (!activeUploadsByBranch.TryGetValue(
                    branch.Id,
                    out var upload))
            {
                results.Add(
                    EmptyBranch(
                        branch.Id,
                        branch.BranchCode,
                        branch.BranchNameArabic,
                        false));

                continue;
            }

            rowsByUpload.TryGetValue(
                upload.Id,
                out var rows);

            rows ??=
                new List<FinancialAccountRow>();

            /*
             * Find the branch-specific parent dynamically.
             *
             * We do NOT hardcode 36002.
             *
             * Among rows below root 36,
             * the direct branch parent is the shortest
             * account code in that hierarchy.
             *
             * Example:
             * 36
             * 36002       <- parent
             * 36002001    <- child
             * 36002002    <- child
             */
            var parent =
                FindBranchExpenseParent(rows);

            if (parent is null)
            {
                results.Add(
                    EmptyBranch(
                        branch.Id,
                        branch.BranchCode,
                        branch.BranchNameArabic,
                        true));

                continue;
            }

            /*
             * IMPORTANT:
             *
             * Branch Expense Total = Parent Balance.
             *
             * Never:
             * Parent + Children
             *
             * because children are already included
             * in the parent and that would double count.
             */
            var totalExpenses =
                parent.Balance ?? 0;

            results.Add(
                new BranchExpenseDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasIncomeStatement =
                        true,

                    HasExpenseAccount =
                        true,

                    ExpenseAccountCode =
                        parent.AccountCode,

                    ExpenseAccountName =
                        parent.AccountName,

                    TotalExpenses =
                        totalExpenses
                });
        }

        var branchesWithData =
            results
                .Where(
                    x =>
                        x.HasIncomeStatement &&
                        x.HasExpenseAccount)
                .ToList();

        var total =
            branchesWithData
                .Sum(x => x.TotalExpenses);

        var highest =
            branchesWithData
                .OrderByDescending(
                    x => x.TotalExpenses)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var lowest =
            branchesWithData
                .OrderBy(
                    x => x.TotalExpenses)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        return new ExpensesAnalysisDto
        {
            FinancialPeriodId =
                period.Id,

            StartDate =
                period.StartDate,

            EndDate =
                period.EndDate,

            Status =
                period.Status,

            ActiveBranches =
                branches.Count,

            BranchesWithData =
                branchesWithData.Count,

            BranchesWithoutData =
                branches.Count -
                branchesWithData.Count,

            AllBranchesExpenses =
                total,

            AverageExpenses =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        total /
                        branchesWithData.Count,
                        2),

            HighestBranch =
                ToRanking(highest),

            LowestBranch =
                ToRanking(lowest),

            Branches =
                results
        };
    }

    private static FinancialAccountRow? FindBranchExpenseParent(
        IReadOnlyCollection<FinancialAccountRow> rows)
    {
        return rows
            .Where(
                x =>
                    x.AccountCode != ExpenseRootCode &&
                    x.AccountCode.StartsWith(
                        ExpenseRootCode,
                        StringComparison.Ordinal))
            .OrderBy(
                x => x.AccountCode.Length)
            .ThenBy(
                x => x.RowOrder)
            .FirstOrDefault();
    }

    private static BranchExpenseDto EmptyBranch(
        int branchId,
        string branchCode,
        string branchName,
        bool hasIncomeStatement)
    {
        return new BranchExpenseDto
        {
            BranchId =
                branchId,

            BranchCode =
                branchCode,

            BranchName =
                branchName,

            HasIncomeStatement =
                hasIncomeStatement,

            HasExpenseAccount =
                false,

            ExpenseAccountCode =
                null,

            ExpenseAccountName =
                null,

            TotalExpenses =
                0
        };
    }

    private static ExpenseBranchRankingDto? ToRanking(
        BranchExpenseDto? branch)
    {
        if (branch is null)
            return null;

        return new ExpenseBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            TotalExpenses =
                branch.TotalExpenses
        };
    }
}
