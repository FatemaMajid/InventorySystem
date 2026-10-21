using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialDashboard;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialDashboard;

public class GetFinancialDashboardQueryHandler
    : IRequestHandler<
        GetFinancialDashboardQuery,
        FinancialDashboardDto?>
{
    private readonly IApplicationDbContext _context;

    public GetFinancialDashboardQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialDashboardDto?> Handle(
        GetFinancialDashboardQuery request,
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
                .OrderBy(x => x.BranchCode)
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            x.BranchCode
                        })
                .ToListAsync(
                    cancellationToken);

        var activeBranches =
            branches.Count;

        var expectedFiles =
            2 + (activeBranches * 4);

        var validUploads =
            await _context.FinancialUploads
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid)
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            x.FileType,
                            x.BranchId,
                            x.Gender
                        })
                .ToListAsync(
                    cancellationToken);

        var validFiles =
            validUploads.Count;

        var missingFiles =
            Math.Max(
                0,
                expectedFiles - validFiles);

        var completionPercentage =
            expectedFiles == 0
                ? 0
                : Math.Round(
                    (decimal)validFiles /
                    expectedFiles *
                    100m,
                    2);

        var completeBranches = 0;

        foreach (var branch in branches)
        {
            var branchUploads =
                validUploads
                    .Where(
                        x =>
                            x.BranchId ==
                                branch.Id)
                    .ToList();

            var hasIncomeStatement =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.IncomeStatement);

            var hasFinancialPosition =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.FinancialPosition);

            var hasSettlementMale =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.Settlement &&
                        x.Gender ==
                            FinancialGender.Male);

            var hasSettlementFemale =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.Settlement &&
                        x.Gender ==
                            FinancialGender.Female);

            if (hasIncomeStatement &&
                hasFinancialPosition &&
                hasSettlementMale &&
                hasSettlementFemale)
            {
                completeBranches++;
            }
        }

        var incompleteBranches =
            activeBranches -
            completeBranches;

        var incomeStatementUploads =
            validUploads.Count(
                x =>
                    x.FileType ==
                        FinancialFileType.IncomeStatement);

        var financialPositionUploads =
            validUploads.Count(
                x =>
                    x.FileType ==
                        FinancialFileType.FinancialPosition);

        var transferUploads =
            validUploads.Count(
                x =>
                    x.FileType ==
                        FinancialFileType.Transfer);

        var settlementUploads =
            validUploads.Count(
                x =>
                    x.FileType ==
                        FinancialFileType.Settlement);

        var transferUploadIds =
            validUploads
                .Where(
                    x =>
                        x.FileType ==
                            FinancialFileType.Transfer)
                .Select(x => x.Id)
                .ToList();

        var transferMaleTotal =
            await _context.TransferRecords
                .AsNoTracking()
                .Where(
                    x =>
                        transferUploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.Gender ==
                            FinancialGender.Male)
                .SumAsync(
                    x =>
                        (decimal?)x.Value,
                    cancellationToken)
                ?? 0;

        var transferFemaleTotal =
            await _context.TransferRecords
                .AsNoTracking()
                .Where(
                    x =>
                        transferUploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.Gender ==
                            FinancialGender.Female)
                .SumAsync(
                    x =>
                        (decimal?)x.Value,
                    cancellationToken)
                ?? 0;

        var settlementUploadIds =
            validUploads
                .Where(
                    x =>
                        x.FileType ==
                            FinancialFileType.Settlement)
                .Select(x => x.Id)
                .ToList();

        var settlementDebitTotal =
            await _context.SettlementSummaries
                .AsNoTracking()
                .Where(
                    x =>
                        settlementUploadIds.Contains(
                            x.FinancialUploadId))
                .SumAsync(
                    x =>
                        (decimal?)x.PeriodDebit,
                    cancellationToken)
                ?? 0;

        var settlementCreditTotal =
            await _context.SettlementSummaries
                .AsNoTracking()
                .Where(
                    x =>
                        settlementUploadIds.Contains(
                            x.FinancialUploadId))
                .SumAsync(
                    x =>
                        (decimal?)x.PeriodCredit,
                    cancellationToken)
                ?? 0;

        var settlementBalanceTotal =
            await _context.SettlementSummaries
                .AsNoTracking()
                .Where(
                    x =>
                        settlementUploadIds.Contains(
                            x.FinancialUploadId))
                .SumAsync(
                    x =>
                        (decimal?)x.PeriodBalance,
                    cancellationToken)
                ?? 0;

        return new FinancialDashboardDto
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
                activeBranches,

            ExpectedFiles =
                expectedFiles,

            ValidFiles =
                validFiles,

            MissingFiles =
                missingFiles,

            CompletionPercentage =
                completionPercentage,

            CompleteBranches =
                completeBranches,

            IncompleteBranches =
                incompleteBranches,

            IncomeStatementUploads =
                incomeStatementUploads,

            FinancialPositionUploads =
                financialPositionUploads,

            TransferUploads =
                transferUploads,

            SettlementUploads =
                settlementUploads,

            TransferMaleTotal =
                transferMaleTotal,

            TransferFemaleTotal =
                transferFemaleTotal,

            SettlementDebitTotal =
                settlementDebitTotal,

            SettlementCreditTotal =
                settlementCreditTotal,

            SettlementBalanceTotal =
                settlementBalanceTotal
        };
    }
}
