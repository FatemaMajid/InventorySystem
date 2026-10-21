using System.Globalization;
using System.Text;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Suppliers;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Suppliers.Queries.GetSuppliersAnalysis;

public sealed class GetSuppliersAnalysisQueryHandler
    : IRequestHandler<
        GetSuppliersAnalysisQuery,
        SuppliersAnalysisDto?>
{
    private const string SupplierRootCode =
        "264";

    private readonly IApplicationDbContext _context;

    public GetSuppliersAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuppliersAnalysisDto?> Handle(
        GetSuppliersAnalysisQuery request,
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

        var supplierRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.AccountCode.StartsWith(
                            SupplierRootCode))
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var rowsByUpload =
            supplierRows
                .GroupBy(
                    x => x.FinancialUploadId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group.ToList());

        var results =
            new List<BranchSuppliersDto>();

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

            var supplierParent =
                FindSupplierParent(
                    rows,
                    branch.BranchNameArabic);

            if (supplierParent is null)
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
             * BUSINESS RULE:
             *
             * Supplier Parent = Branch Total.
             *
             * Children are details only.
             *
             * Never sum:
             * Parent + Children.
             *
             * Supplier value comes directly from
             * Liabilities in Financial Position.
             */
            var suppliersTotal =
                supplierParent.Liabilities ?? 0;

            results.Add(
                new BranchSuppliersDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasFinancialPosition =
                        true,

                    HasSupplierAccount =
                        true,

                    SupplierAccountCode =
                        supplierParent.AccountCode,

                    SupplierAccountName =
                        supplierParent.AccountName,

                    SuppliersTotal =
                        suppliersTotal
                });
        }

        var branchesWithData =
            results
                .Where(
                    x =>
                        x.HasFinancialPosition &&
                        x.HasSupplierAccount)
                .ToList();

        var total =
            branchesWithData
                .Sum(
                    x => x.SuppliersTotal);

        var highest =
            branchesWithData
                .OrderByDescending(
                    x => x.SuppliersTotal)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var lowest =
            branchesWithData
                .OrderBy(
                    x => x.SuppliersTotal)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        return new SuppliersAnalysisDto
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

            AllBranchesSuppliers =
                total,

            AverageSuppliers =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        total /
                        branchesWithData.Count,
                        2),

            HighestSuppliersBranch =
                ToRanking(
                    highest),

            LowestSuppliersBranch =
                ToRanking(
                    lowest),

            Branches =
                results
        };
    }

    private static FinancialAccountRow? FindSupplierParent(
        IReadOnlyCollection<FinancialAccountRow> rows,
        string branchName)
    {
        var branchToken =
            NormalizeBranchName(
                branchName);

        /*
         * Example:
         *
         * 264       موردون الفروع
         * 264002    موردون الجمعية      <- Parent
         * 264002001 بيت الخيط...        <- Child
         *
         * Parent is selected from the
         * current branch FinancialPosition.
         */
        return rows
            .Where(
                x =>
                    x.AccountCode !=
                        SupplierRootCode &&
                    x.AccountCode.StartsWith(
                        SupplierRootCode,
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
                "موردون",
                "موردين",
                "الموردون",
                "الموردين"
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

    private static BranchSuppliersDto EmptyBranch(
        int branchId,
        string branchCode,
        string branchName,
        bool hasFinancialPosition)
    {
        return new BranchSuppliersDto
        {
            BranchId =
                branchId,

            BranchCode =
                branchCode,

            BranchName =
                branchName,

            HasFinancialPosition =
                hasFinancialPosition,

            HasSupplierAccount =
                false,

            SupplierAccountCode =
                null,

            SupplierAccountName =
                null,

            SuppliersTotal =
                0
        };
    }

    private static SupplierBranchRankingDto? ToRanking(
        BranchSuppliersDto? branch)
    {
        if (branch is null)
            return null;

        return new SupplierBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            SuppliersTotal =
                branch.SuppliersTotal
        };
    }
}
