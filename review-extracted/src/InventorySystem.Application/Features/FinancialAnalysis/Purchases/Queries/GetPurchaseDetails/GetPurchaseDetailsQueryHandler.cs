using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Purchases;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Purchases.Queries.GetPurchaseDetails;

public sealed class GetPurchaseDetailsQueryHandler
    : IRequestHandler<
        GetPurchaseDetailsQuery,
        BranchPurchaseDetailsDto?>
{
    private const string PurchasesAccountCode =
        "3309";

    private readonly IApplicationDbContext _context;

    public GetPurchaseDetailsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BranchPurchaseDetailsDto?> Handle(
        GetPurchaseDetailsQuery request,
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

        if(!periodExists)
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

        if(branch is null)
            return null;

        var upload =
            await _context.FinancialUploads
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.BranchId ==
                            request.BranchId &&
                        x.FileType ==
                            FinancialFileType.IncomeStatement &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid)
                .OrderByDescending(
                    x =>
                        x.Version)
                .ThenByDescending(
                    x =>
                        x.Id)
                .Select(
                    x =>
                        new
                        {
                            x.Id
                        })
                .FirstOrDefaultAsync(
                    cancellationToken);

        if(upload is null)
            return null;

        var row =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialUploadId ==
                            upload.Id &&
                        x.AccountCode ==
                            PurchasesAccountCode)
                .OrderBy(
                    x =>
                        x.RowOrder)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if(row is null)
        {
            return new BranchPurchaseDetailsDto
            {
                FinancialPeriodId =
                    request.FinancialPeriodId,

                BranchId =
                    branch.Id,

                BranchCode =
                    branch.BranchCode,

                BranchName =
                    branch.BranchNameArabic,

                TotalPurchases =
                    0
            };
        }

        var purchases =
            GetAmount(
                row);

        return new BranchPurchaseDetailsDto
        {
            FinancialPeriodId =
                request.FinancialPeriodId,

            BranchId =
                branch.Id,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchNameArabic,

            TotalPurchases =
                purchases,

            Details =
            {
                new PurchaseDetailRowDto
                {
                    AccountCode =
                        row.AccountCode,

                    AccountName =
                        row.AccountName,

                    Debit =
                        row.Debit ??
                        0,

                    Credit =
                        row.Credit ??
                        0,

                    Balance =
                        row.Balance ??
                        0,

                    Purchases =
                        purchases,

                    RowOrder =
                        row.RowOrder
                }
            }
        };
    }

    private static decimal GetAmount(
        InventorySystem.Domain.FinancialAnalysis.Entities.FinancialAccountRow row)
    {
        if(row.Balance.HasValue)
        {
            return Math.Abs(
                row.Balance.Value);
        }

        if(
            row.Debit.HasValue &&
            !row.Credit.HasValue)
        {
            return Math.Abs(
                row.Debit.Value);
        }

        if(
            !row.Debit.HasValue &&
            row.Credit.HasValue)
        {
            return Math.Abs(
                row.Credit.Value);
        }

        if(
            row.Debit.HasValue &&
            row.Credit.HasValue)
        {
            return Math.Abs(
                row.Debit.Value -
                row.Credit.Value);
        }

        return 0;
    }
}
