using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Capital;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Capital.Queries.GetCapitalAnalysis;

public sealed class GetCapitalAnalysisQueryHandler
    : IRequestHandler<
        GetCapitalAnalysisQuery,
        CapitalAnalysisDto?>
{
    private const string CapitalRootCode =
        "29";

    private readonly IApplicationDbContext _context;

    public GetCapitalAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CapitalAnalysisDto?> Handle(
        GetCapitalAnalysisQuery request,
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
                            FinancialFileType.FinancialPosition &&
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

        var uploadIds =
            activeUploadsByBranch
                .Values
                .Select(x => x.Id)
                .ToList();

        var capitalRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.AccountCode.StartsWith(
                            CapitalRootCode))
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var rowsByUpload =
            capitalRows
                .GroupBy(
                    x => x.FinancialUploadId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group.ToList());

        var results =
            new List<BranchCapitalDto>();

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
             * Root:
             * 29 = Branch Capital
             *
             * The branch capital account is
             * the direct child under 29 inside
             * this branch FinancialPosition.
             *
             * Example:
             *
             * 29
             * 29004 <- Association branch capital
             *
             * We do not hardcode 29004.
             */
            var capitalAccount =
                rows
                    .Where(
                        x =>
                            x.AccountCode !=
                                CapitalRootCode &&
                            x.AccountCode.StartsWith(
                                CapitalRootCode,
                                StringComparison.Ordinal))
                    .OrderBy(
                        x => x.AccountCode.Length)
                    .ThenBy(
                        x => x.RowOrder)
                    .FirstOrDefault();

            if (capitalAccount is null)
            {
                results.Add(
                    EmptyBranch(
                        branch.Id,
                        branch.BranchCode,
                        branch.BranchNameArabic,
                        true));

                continue;
            }

            var assets =
                capitalAccount.Assets ?? 0;

            var liabilities =
                capitalAccount.Liabilities ?? 0;

            /*
             * Business Rule:
             *
             * Assets      => positive
             * Liabilities => negative
             *
             * Capital =
             * Assets - Liabilities
             */
            var capital =
                assets -
                liabilities;

            results.Add(
                new BranchCapitalDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasFinancialPosition =
                        true,

                    HasCapitalAccount =
                        true,

                    CapitalAccountCode =
                        capitalAccount.AccountCode,

                    CapitalAccountName =
                        capitalAccount.AccountName,

                    Assets =
                        assets,

                    Liabilities =
                        liabilities,

                    Capital =
                        capital
                });
        }

        var branchesWithData =
            results
                .Where(
                    x =>
                        x.HasFinancialPosition &&
                        x.HasCapitalAccount)
                .ToList();

        var total =
            branchesWithData
                .Sum(
                    x => x.Capital);

        var highest =
            branchesWithData
                .OrderByDescending(
                    x => x.Capital)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var lowest =
            branchesWithData
                .OrderBy(
                    x => x.Capital)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        return new CapitalAnalysisDto
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

            AllBranchesCapital =
                total,

            AverageCapital =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        total /
                        branchesWithData.Count,
                        2),

            HighestCapitalBranch =
                ToRanking(
                    highest),

            LowestCapitalBranch =
                ToRanking(
                    lowest),

            Branches =
                results
        };
    }

    private static BranchCapitalDto EmptyBranch(
        int branchId,
        string branchCode,
        string branchName,
        bool hasFinancialPosition)
    {
        return new BranchCapitalDto
        {
            BranchId =
                branchId,

            BranchCode =
                branchCode,

            BranchName =
                branchName,

            HasFinancialPosition =
                hasFinancialPosition,

            HasCapitalAccount =
                false,

            CapitalAccountCode =
                null,

            CapitalAccountName =
                null,

            Assets =
                0,

            Liabilities =
                0,

            Capital =
                0
        };
    }

    private static CapitalBranchRankingDto? ToRanking(
        BranchCapitalDto? branch)
    {
        if (branch is null)
            return null;

        return new CapitalBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            Capital =
                branch.Capital
        };
    }
}
