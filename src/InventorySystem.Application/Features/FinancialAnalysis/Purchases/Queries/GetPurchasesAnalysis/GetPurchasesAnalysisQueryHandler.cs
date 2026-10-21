using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Purchases;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Purchases.Queries.GetPurchasesAnalysis;

public sealed class GetPurchasesAnalysisQueryHandler
    : IRequestHandler<
        GetPurchasesAnalysisQuery,
        PurchasesAnalysisDto?>
{
    private const string PurchasesAccountCode =
        "3309";

    private readonly IApplicationDbContext _context;

    public GetPurchasesAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PurchasesAnalysisDto?> Handle(
        GetPurchasesAnalysisQuery request,
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

        var purchaseRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        activeUploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.AccountCode ==
                            PurchasesAccountCode)
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var purchaseRowByUpload =
            purchaseRows
                .GroupBy(
                    x => x.FinancialUploadId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group
                            .OrderBy(
                                x => x.RowOrder)
                            .First());

        var results =
            new List<BranchPurchasesDto>();

        foreach (var branch in branches)
        {
            if (!activeUploadsByBranch
                    .TryGetValue(
                        branch.Id,
                        out var upload))
            {
                results.Add(
                    new BranchPurchasesDto
                    {
                        BranchId =
                            branch.Id,

                        BranchCode =
                            branch.BranchCode,

                        BranchName =
                            branch.BranchNameArabic,

                        HasIncomeStatement =
                            false,

                        HasPurchasesAccount =
                            false,

                        Purchases =
                            0
                    });

                continue;
            }

            purchaseRowByUpload.TryGetValue(
                upload.Id,
                out var row);

            if (row is null)
            {
                results.Add(
                    new BranchPurchasesDto
                    {
                        BranchId =
                            branch.Id,

                        BranchCode =
                            branch.BranchCode,

                        BranchName =
                            branch.BranchNameArabic,

                        HasIncomeStatement =
                            true,

                        HasPurchasesAccount =
                            false,

                        Purchases =
                            0
                    });

                continue;
            }

            var purchases =
                GetAmount(row);

            results.Add(
                new BranchPurchasesDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasIncomeStatement =
                        true,

                    HasPurchasesAccount =
                        true,

                    Purchases =
                        purchases
                });
        }

        var branchesWithData =
            results
                .Where(
                    x =>
                        x.HasIncomeStatement &&
                        x.HasPurchasesAccount)
                .ToList();

        var total =
            branchesWithData
                .Sum(
                    x => x.Purchases);

        var highest =
            branchesWithData
                .OrderByDescending(
                    x => x.Purchases)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var lowest =
            branchesWithData
                .OrderBy(
                    x => x.Purchases)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        return new PurchasesAnalysisDto
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

            AllBranchesTotal =
                total,

            AveragePurchases =
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

    private static decimal GetAmount(
        InventorySystem.Domain.FinancialAnalysis.Entities.FinancialAccountRow row)
    {
        /*
         * Business Rule:
         *
         * Account 3309 = Purchases.
         *
         * Do not use accounts:
         * 337 / 338 / 339 / 340
         *
         * Value extraction:
         * - Prefer Balance when present.
         * - Otherwise use the one available
         *   Debit/Credit value.
         */

        if (row.Balance.HasValue)
        {
            return Math.Abs(
                row.Balance.Value);
        }

        if (row.Debit.HasValue &&
            !row.Credit.HasValue)
        {
            return Math.Abs(
                row.Debit.Value);
        }

        if (!row.Debit.HasValue &&
            row.Credit.HasValue)
        {
            return Math.Abs(
                row.Credit.Value);
        }

        if (row.Debit.HasValue &&
            row.Credit.HasValue)
        {
            return Math.Abs(
                row.Debit.Value -
                row.Credit.Value);
        }

        return 0;
    }

    private static PurchasesBranchRankingDto? ToRanking(
        BranchPurchasesDto? branch)
    {
        if (branch is null)
            return null;

        return new PurchasesBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            Purchases =
                branch.Purchases
        };
    }
}
