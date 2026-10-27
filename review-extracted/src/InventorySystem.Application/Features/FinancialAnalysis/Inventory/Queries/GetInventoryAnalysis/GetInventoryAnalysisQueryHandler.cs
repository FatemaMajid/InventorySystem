using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Inventory;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Inventory.Queries.GetInventoryAnalysis;

public sealed class GetInventoryAnalysisQueryHandler
    : IRequestHandler<
        GetInventoryAnalysisQuery,
        InventoryAnalysisDto?>
{
    private const string InventoryAccountCode =
        "137";

    private readonly IApplicationDbContext _context;

    public GetInventoryAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryAnalysisDto?> Handle(
        GetInventoryAnalysisQuery request,
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

        var inventoryRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        activeUploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.AccountCode ==
                            InventoryAccountCode)
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var inventoryRowByUpload =
            inventoryRows
                .GroupBy(
                    x =>
                        x.FinancialUploadId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group
                            .OrderBy(
                                x => x.RowOrder)
                            .First());

        var branchResults =
            new List<BranchInventoryDto>();

        foreach (var branch in branches)
        {
            if (!activeUploadsByBranch
                    .TryGetValue(
                        branch.Id,
                        out var upload))
            {
                branchResults.Add(
                    new BranchInventoryDto
                    {
                        BranchId =
                            branch.Id,

                        BranchCode =
                            branch.BranchCode,

                        BranchName =
                            branch.BranchNameArabic,

                        HasIncomeStatement =
                            false,

                        HasInventoryAccount =
                            false,

                        OpeningInventory =
                            0,

                        ClosingInventory =
                            0
                    });

                continue;
            }

            inventoryRowByUpload.TryGetValue(
                upload.Id,
                out var inventoryRow);

            if (inventoryRow is null)
            {
                branchResults.Add(
                    new BranchInventoryDto
                    {
                        BranchId =
                            branch.Id,

                        BranchCode =
                            branch.BranchCode,

                        BranchName =
                            branch.BranchNameArabic,

                        HasIncomeStatement =
                            true,

                        HasInventoryAccount =
                            false,

                        OpeningInventory =
                            0,

                        ClosingInventory =
                            0
                    });

                continue;
            }

            /*
             * Business Rule:
             *
             * Account 137
             *
             * Debit  = Opening Inventory
             * Credit = Closing Inventory
             *
             * No Balance formula is used here.
             */

            var openingInventory =
                inventoryRow.Debit ?? 0;

            var closingInventory =
                inventoryRow.Credit ?? 0;

            branchResults.Add(
                new BranchInventoryDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasIncomeStatement =
                        true,

                    HasInventoryAccount =
                        true,

                    OpeningInventory =
                        openingInventory,

                    ClosingInventory =
                        closingInventory
                });
        }

        /*
         * Rankings and averages must only use
         * branches that actually contain
         * Account 137.
         *
         * Missing files / missing account rows
         * must not become false zero-value
         * lowest branches.
         */
        var branchesWithData =
            branchResults
                .Where(
                    x =>
                        x.HasIncomeStatement &&
                        x.HasInventoryAccount)
                .ToList();

        var highestOpening =
            branchesWithData
                .OrderByDescending(
                    x =>
                        x.OpeningInventory)
                .ThenBy(
                    x =>
                        x.BranchCode)
                .FirstOrDefault();

        var lowestOpening =
            branchesWithData
                .OrderBy(
                    x =>
                        x.OpeningInventory)
                .ThenBy(
                    x =>
                        x.BranchCode)
                .FirstOrDefault();

        var highestClosing =
            branchesWithData
                .OrderByDescending(
                    x =>
                        x.ClosingInventory)
                .ThenBy(
                    x =>
                        x.BranchCode)
                .FirstOrDefault();

        var lowestClosing =
            branchesWithData
                .OrderBy(
                    x =>
                        x.ClosingInventory)
                .ThenBy(
                    x =>
                        x.BranchCode)
                .FirstOrDefault();

        var openingTotal =
            branchesWithData
                .Sum(
                    x =>
                        x.OpeningInventory);

        var closingTotal =
            branchesWithData
                .Sum(
                    x =>
                        x.ClosingInventory);

        return new InventoryAnalysisDto
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

            AllBranchesOpeningTotal =
                openingTotal,

            AllBranchesClosingTotal =
                closingTotal,

            InventoryChange =
                closingTotal -
                openingTotal,

            AverageOpeningInventory =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        openingTotal /
                        branchesWithData.Count,
                        2),

            AverageClosingInventory =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        closingTotal /
                        branchesWithData.Count,
                        2),

            HighestOpeningBranch =
                ToRanking(
                    highestOpening,
                    highestOpening?.OpeningInventory
                        ?? 0),

            LowestOpeningBranch =
                ToRanking(
                    lowestOpening,
                    lowestOpening?.OpeningInventory
                        ?? 0),

            HighestClosingBranch =
                ToRanking(
                    highestClosing,
                    highestClosing?.ClosingInventory
                        ?? 0),

            LowestClosingBranch =
                ToRanking(
                    lowestClosing,
                    lowestClosing?.ClosingInventory
                        ?? 0),

            Branches =
                branchResults
        };
    }

    private static InventoryBranchRankingDto? ToRanking(
        BranchInventoryDto? branch,
        decimal value)
    {
        if (branch is null)
            return null;

        return new InventoryBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            Value =
                value
        };
    }
}
