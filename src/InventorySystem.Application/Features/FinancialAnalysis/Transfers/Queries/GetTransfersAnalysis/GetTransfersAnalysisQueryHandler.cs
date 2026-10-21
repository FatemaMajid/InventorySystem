using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Transfers;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Transfers.Queries.GetTransfersAnalysis;

public sealed class GetTransfersAnalysisQueryHandler
    : IRequestHandler<
        GetTransfersAnalysisQuery,
        TransfersAnalysisDto?>
{
    private readonly IApplicationDbContext _context;

    public GetTransfersAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TransfersAnalysisDto?> Handle(
        GetTransfersAnalysisQuery request,
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

        /*
         * Transfers have two global files:
         *
         * Male
         * Female
         *
         * They are period-level uploads,
         * not one upload per branch.
         *
         * Use only latest active valid upload
         * for each gender.
         */
        var transferUploads =
            await _context.FinancialUploads
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.FileType ==
                            FinancialFileType.Transfer &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid &&
                        x.Gender.HasValue)
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            Gender =
                                x.Gender!.Value,
                            x.Version
                        })
                .ToListAsync(
                    cancellationToken);

        var maleUpload =
            transferUploads
                .Where(
                    x =>
                        x.Gender ==
                            FinancialGender.Male)
                .OrderByDescending(
                    x => x.Version)
                .ThenByDescending(
                    x => x.Id)
                .FirstOrDefault();

        var femaleUpload =
            transferUploads
                .Where(
                    x =>
                        x.Gender ==
                            FinancialGender.Female)
                .OrderByDescending(
                    x => x.Version)
                .ThenByDescending(
                    x => x.Id)
                .FirstOrDefault();

        var transferUploadIds =
            new List<int>();

        if (maleUpload is not null)
        {
            transferUploadIds.Add(
                maleUpload.Id);
        }

        if (femaleUpload is not null)
        {
            transferUploadIds.Add(
                femaleUpload.Id);
        }

        var records =
            await _context.TransferRecords
                .AsNoTracking()
                .Where(
                    x =>
                        transferUploadIds.Contains(
                            x.FinancialUploadId) &&
                        branchIds.Contains(
                            x.BranchId))
                .OrderBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var maleRecords =
            maleUpload is null
                ? new Dictionary<int,
                    InventorySystem.Domain.FinancialAnalysis.Entities.TransferRecord>()
                : records
                    .Where(
                        x =>
                            x.FinancialUploadId ==
                                maleUpload.Id &&
                            x.Gender ==
                                FinancialGender.Male)
                    .ToDictionary(
                        x => x.BranchId);

        var femaleRecords =
            femaleUpload is null
                ? new Dictionary<int,
                    InventorySystem.Domain.FinancialAnalysis.Entities.TransferRecord>()
                : records
                    .Where(
                        x =>
                            x.FinancialUploadId ==
                                femaleUpload.Id &&
                            x.Gender ==
                                FinancialGender.Female)
                    .ToDictionary(
                        x => x.BranchId);

        var branchResults =
            new List<BranchTransferDto>();

        foreach (var branch in branches)
        {
            maleRecords.TryGetValue(
                branch.Id,
                out var male);

            femaleRecords.TryGetValue(
                branch.Id,
                out var female);

            var maleValue =
                male?.Value ?? 0;

            var femaleValue =
                female?.Value ?? 0;

            branchResults.Add(
                new BranchTransferDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasMaleTransfer =
                        male is not null,

                    HasFemaleTransfer =
                        female is not null,

                    /*
                     * TransferRecord.Value is used
                     * exactly as parsed from:
                     *
                     * Debit - Credit
                     *
                     * No Math.Abs().
                     * No sign conversion.
                     */
                    MaleTransfer =
                        maleValue,

                    FemaleTransfer =
                        femaleValue,

                    TotalTransfer =
                        maleValue +
                        femaleValue
                });
        }

        var branchesWithAnyData =
            branchResults
                .Where(
                    x =>
                        x.HasMaleTransfer ||
                        x.HasFemaleTransfer)
                .ToList();

        /*
         * Rankings compare only complete branches.
         *
         * A branch missing Male or Female transfer
         * must not become a false lowest branch.
         */
        var completeBranches =
            branchResults
                .Where(
                    x =>
                        x.HasCompleteData)
                .ToList();

        var highest =
            completeBranches
                .OrderByDescending(
                    x => x.TotalTransfer)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var lowest =
            completeBranches
                .OrderBy(
                    x => x.TotalTransfer)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var allMale =
            maleRecords
                .Values
                .Sum(
                    x => x.Value);

        var allFemale =
            femaleRecords
                .Values
                .Sum(
                    x => x.Value);

        var allTotal =
            allMale +
            allFemale;

        var completeTotal =
            completeBranches
                .Sum(
                    x => x.TotalTransfer);

        return new TransfersAnalysisDto
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

            HasMaleTransferFile =
                maleUpload is not null,

            HasFemaleTransferFile =
                femaleUpload is not null,

            MaleRecordsCount =
                maleRecords.Count,

            FemaleRecordsCount =
                femaleRecords.Count,

            BranchesWithAnyData =
                branchesWithAnyData.Count,

            BranchesWithCompleteData =
                completeBranches.Count,

            BranchesWithoutCompleteData =
                branches.Count -
                completeBranches.Count,

            AllBranchesMaleTransfers =
                allMale,

            AllBranchesFemaleTransfers =
                allFemale,

            AllBranchesTotalTransfers =
                allTotal,

            AverageCompleteBranchTransfers =
                completeBranches.Count == 0
                    ? 0
                    : Math.Round(
                        completeTotal /
                        completeBranches.Count,
                        2),

            HighestTransferBranch =
                ToRanking(
                    highest),

            LowestTransferBranch =
                ToRanking(
                    lowest),

            Branches =
                branchResults
        };
    }

    private static TransferBranchRankingDto? ToRanking(
        BranchTransferDto? branch)
    {
        if (branch is null)
            return null;

        return new TransferBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            MaleTransfer =
                branch.MaleTransfer,

            FemaleTransfer =
                branch.FemaleTransfer,

            TotalTransfer =
                branch.TotalTransfer
        };
    }
}
