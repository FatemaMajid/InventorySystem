using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Sales;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Sales.Queries.GetSalesAnalysis;

public sealed class GetSalesAnalysisQueryHandler
    : IRequestHandler<
        GetSalesAnalysisQuery,
        SalesAnalysisDto?>
{
    private const string SalesCode =
        "416";

    private const string SalesReturnsCode =
        "417";

    private const string DiscountAllowedCode =
        "415";

    private const string BranchDiscountAllowedCode =
        "418";

    private static readonly string[] SalesAccountCodes =
    {
        SalesCode,
        SalesReturnsCode,
        DiscountAllowedCode,
        BranchDiscountAllowedCode
    };

    private readonly IApplicationDbContext _context;

    public GetSalesAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesAnalysisDto?> Handle(
        GetSalesAnalysisQuery request,
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

        // Financial branches only.
        // Keep this filter consistent with Upload,
        // Dashboard and Parser logic.
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
                    x =>
                        x.BranchCode)
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

        // Only the active valid Income Statement
        // upload can participate in analysis.
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

        // Defensive handling:
        // there should be only one active upload
        // per branch logical slot.
        // If old inconsistent data exists,
        // use the latest active version.
        var activeUploadsByBranch =
            uploads
                .GroupBy(
                    x => x.BranchId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group
                            .OrderByDescending(
                                x => x.Version)
                            .ThenByDescending(
                                x => x.Id)
                            .First());

        var activeUploadIds =
            activeUploadsByBranch
                .Values
                .Select(x => x.Id)
                .ToList();

        var accountRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        activeUploadIds.Contains(
                            x.FinancialUploadId) &&
                        SalesAccountCodes.Contains(
                            x.AccountCode))
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var rowsByUpload =
            accountRows
                .GroupBy(
                    x =>
                        x.FinancialUploadId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group.ToList());

        var branchResults =
            new List<BranchSalesDto>();

        foreach (var branch in branches)
        {
            if (!activeUploadsByBranch
                    .TryGetValue(
                        branch.Id,
                        out var upload))
            {
                // Requirement:
                // all active financial branches
                // must appear even when their file
                // has not been uploaded yet.
                branchResults.Add(
                    new BranchSalesDto
                    {
                        BranchId =
                            branch.Id,

                        BranchCode =
                            branch.BranchCode,

                        BranchName =
                            branch.BranchNameArabic,

                        HasIncomeStatement =
                            false,

                        Sales =
                            0,

                        SalesReturns =
                            0,

                        DiscountAllowed =
                            0,

                        BranchDiscountAllowed =
                            0,

                        NetSales =
                            0
                    });

                continue;
            }

            rowsByUpload.TryGetValue(
                upload.Id,
                out var rows);

            rows ??=
                new List<FinancialAccountRow>();

            var sales =
                GetAccountAmount(
                    rows,
                    SalesCode);

            var salesReturns =
                GetAccountAmount(
                    rows,
                    SalesReturnsCode);

            var discountAllowed =
                GetAccountAmount(
                    rows,
                    DiscountAllowedCode);

            var branchDiscountAllowed =
                GetAccountAmount(
                    rows,
                    BranchDiscountAllowedCode);

            var netSales =
                sales
                - salesReturns
                - discountAllowed
                - branchDiscountAllowed;

            branchResults.Add(
                new BranchSalesDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasIncomeStatement =
                        true,

                    Sales =
                        sales,

                    SalesReturns =
                        salesReturns,

                    DiscountAllowed =
                        discountAllowed,

                    BranchDiscountAllowed =
                        branchDiscountAllowed,

                    NetSales =
                        netSales
                });
        }

        // Highest / Lowest are calculated only
        // from branches that actually have
        // a valid Income Statement.
        // Missing branches must not become
        // the false "lowest" branch with zero.
        var branchesWithData =
            branchResults
                .Where(
                    x =>
                        x.HasIncomeStatement)
                .ToList();

        var highest =
            branchesWithData
                .OrderByDescending(
                    x =>
                        x.NetSales)
                .ThenBy(
                    x =>
                        x.BranchCode)
                .FirstOrDefault();

        var lowest =
            branchesWithData
                .OrderBy(
                    x =>
                        x.NetSales)
                .ThenBy(
                    x =>
                        x.BranchCode)
                .FirstOrDefault();

        var totalNetSales =
            branchesWithData
                .Sum(
                    x =>
                        x.NetSales);

        return new SalesAnalysisDto
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

            AllBranchesSales =
                branchesWithData.Sum(
                    x =>
                        x.Sales),

            AllBranchesSalesReturns =
                branchesWithData.Sum(
                    x =>
                        x.SalesReturns),

            AllBranchesDiscountAllowed =
                branchesWithData.Sum(
                    x =>
                        x.DiscountAllowed),

            AllBranchesBranchDiscountAllowed =
                branchesWithData.Sum(
                    x =>
                        x.BranchDiscountAllowed),

            // This is the required
            // AllBranchesTotal:
            // Sum of branch NetSales.
            AllBranchesTotal =
                totalNetSales,

            AverageNetSales =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        totalNetSales /
                        branchesWithData.Count,
                        2),

            HighestBranch =
                ToRanking(
                    highest),

            LowestBranch =
                ToRanking(
                    lowest),

            // Already ordered by BranchCode ASC.
            Branches =
                branchResults
        };
    }

    private static SalesBranchRankingDto? ToRanking(
        BranchSalesDto? branch)
    {
        if (branch is null)
            return null;

        return new SalesBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            NetSales =
                branch.NetSales
        };
    }

    private static decimal GetAccountAmount(
        IReadOnlyCollection<FinancialAccountRow> rows,
        string accountCode)
    {
        var row =
            rows.FirstOrDefault(
                x =>
                    x.AccountCode ==
                        accountCode);

        if (row is null)
            return 0;

        /*
         * Business Analysis Rule:
         *
         * Source accounting values remain untouched
         * in FinancialAccountRows.
         *
         * Sales accounts can naturally have a negative
         * Balance because sales are recorded on Credit.
         *
         * For the Sales KPI we need the monetary magnitude:
         *
         * 416 Sales              -> positive amount
         * 417 Sales Returns      -> positive deduction
         * 415 Discount Allowed   -> positive deduction
         * 418 Branch Discount    -> positive deduction
         *
         * NetSales =
         * Sales
         * - SalesReturns
         * - DiscountAllowed
         * - BranchDiscountAllowed
         */

        if (row.Balance.HasValue)
        {
            return Math.Abs(
                row.Balance.Value);
        }

        var hasDebit =
            row.Debit.HasValue;

        var hasCredit =
            row.Credit.HasValue;

        if (hasDebit &&
            !hasCredit)
        {
            return Math.Abs(
                row.Debit!.Value);
        }

        if (!hasDebit &&
            hasCredit)
        {
            return Math.Abs(
                row.Credit!.Value);
        }

        if (hasDebit &&
            hasCredit)
        {
            return Math.Abs(
                row.Debit!.Value -
                row.Credit!.Value);
        }

        return 0;
    }
}
