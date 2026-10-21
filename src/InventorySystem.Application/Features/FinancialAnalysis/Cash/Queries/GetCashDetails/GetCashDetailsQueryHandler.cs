using System.Globalization;
using System.Text;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Cash;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Cash.Queries.GetCashDetails;

public sealed class GetCashDetailsQueryHandler
    : IRequestHandler<
        GetCashDetailsQuery,
        BranchCashDetailsDto?>
{
    private const string CashRootCode =
        "1801";

    private readonly IApplicationDbContext _context;

    public GetCashDetailsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BranchCashDetailsDto?> Handle(
        GetCashDetailsQuery request,
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
                            FinancialFileType.FinancialPosition &&
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
                    x => new
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
                            CashRootCode))
                .OrderBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var branchToken =
            NormalizeBranchName(
                branch.BranchNameArabic);

        var cashGroup =
            rows
                .Where(
                    x =>
                        x.AccountCode !=
                            CashRootCode &&
                        x.AccountCode.StartsWith(
                            CashRootCode,
                            StringComparison.Ordinal) &&
                        NormalizeBranchName(
                            x.AccountName)
                            .Contains(
                                branchToken,
                                StringComparison.Ordinal))
                .OrderBy(
                    x => x.AccountCode.Length)
                .ThenBy(
                    x => x.RowOrder)
                .FirstOrDefault();

        if (cashGroup is null)
            return null;

        var cashboxes =
            rows
                .Where(
                    x =>
                        x.AccountCode !=
                            cashGroup.AccountCode &&
                        x.AccountCode.StartsWith(
                            cashGroup.AccountCode,
                            StringComparison.Ordinal))
                .OrderBy(
                    x => x.RowOrder)
                .Select(
                    x =>
                        new CashboxDetailDto
                        {
                            AccountCode =
                                x.AccountCode,

                            AccountName =
                                x.AccountName,

                            Value =
                                x.Assets ?? 0,

                            RowOrder =
                                x.RowOrder
                        })
                .ToList();

        return new BranchCashDetailsDto
        {
            FinancialPeriodId =
                request.FinancialPeriodId,

            BranchId =
                branch.Id,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchNameArabic,

            CashGroupAccountCode =
                cashGroup.AccountCode,

            CashGroupAccountName =
                cashGroup.AccountName,

            TotalCash =
                cashboxes.Sum(
                    x => x.Value),

            CashboxesCount =
                cashboxes.Count,

            Cashboxes =
                cashboxes
        };
    }

    private static string NormalizeBranchName(
        string value)
    {
        var normalized =
            NormalizeArabic(
                value);

        var wordsToRemove =
            new[]
            {
                "فرع",
                "صناديق",
                "صندوق",
                "مركز",
                "كاشير",
                "مردود",
                "الخردة",
                "خردة",
                "كي"
            };

        foreach (var word in wordsToRemove)
        {
            normalized =
                normalized.Replace(
                    NormalizeArabic(word),
                    " ",
                    StringComparison.Ordinal);
        }

        return string.Join(
            ' ',
            normalized.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries));
    }

    private static string NormalizeArabic(
        string value)
    {
        if (string.IsNullOrWhiteSpace(
                value))
        {
            return string.Empty;
        }

        var normalized =
            value
                .Trim()
                .Replace("ـ","")
                .Replace("أ","ا")
                .Replace("إ","ا")
                .Replace("آ","ا")
                .Replace("ى","ي")
                .Replace("ؤ","و")
                .Replace("ئ","ي")
                .Replace("-"," ")
                .Replace("_"," ");

        var builder =
            new StringBuilder();

        foreach (var character in normalized)
        {
            var category =
                CharUnicodeInfo.GetUnicodeCategory(
                    character);

            if (category !=
                    UnicodeCategory.NonSpacingMark &&
                category !=
                    UnicodeCategory.Format)
            {
                builder.Append(
                    character);
            }
        }

        return string.Join(
            ' ',
            builder
                .ToString()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries));
    }
}
