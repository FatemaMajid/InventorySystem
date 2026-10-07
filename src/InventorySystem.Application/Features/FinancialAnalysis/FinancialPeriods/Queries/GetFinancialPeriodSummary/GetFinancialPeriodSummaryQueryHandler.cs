using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialMonitoring;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodSummary;

public class GetFinancialPeriodSummaryQueryHandler
    : IRequestHandler<GetFinancialPeriodSummaryQuery, FinancialPeriodSummaryDto?>
{
    private readonly IApplicationDbContext _context;

    public GetFinancialPeriodSummaryQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialPeriodSummaryDto?> Handle(
        GetFinancialPeriodSummaryQuery request,
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

        var activeBranches =
            await _context.Branches
                .AsNoTracking()
                .CountAsync(
                    x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.BranchCode != "0" &&
                        x.BranchCode != "001" &&
                        x.BranchCode != "002",
                    cancellationToken);

        var expectedFiles =
            2 + (activeBranches * 4);

        var validFiles =
            await _context.FinancialUploads
                .AsNoTracking()
                .CountAsync(
                    x =>
                        x.FinancialPeriodId ==
                            period.Id &&
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.Status ==
                            FinancialUploadStatus.Valid,
                    cancellationToken);

        var missingFiles =
            Math.Max(
                0,
                expectedFiles -
                validFiles);

        var completionPercentage =
            expectedFiles == 0
                ? 0m
                : Math.Round(
                    (decimal)validFiles /
                    expectedFiles *
                    100m,
                    2);

        return new FinancialPeriodSummaryDto
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
                completionPercentage
        };
    }
}
