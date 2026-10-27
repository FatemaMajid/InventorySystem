using System.Globalization;
using System.Text;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Customers;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Customers.Queries.GetCustomersAnalysis;

public sealed class GetCustomersAnalysisQueryHandler
    : IRequestHandler<
        GetCustomersAnalysisQuery,
        CustomersAnalysisDto?>
{
    private const string CustomerRootCode =
        "123";

    private readonly IApplicationDbContext _context;

    public GetCustomersAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomersAnalysisDto?> Handle(
        GetCustomersAnalysisQuery request,
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
                .GroupBy(x => x.BranchId)
                .ToDictionary(
                    x => x.Key,
                    x =>
                        x.OrderByDescending(
                                y => y.Version)
                            .ThenByDescending(
                                y => y.Id)
                            .First());

        var uploadIds =
            activeUploadsByBranch
                .Values
                .Select(x => x.Id)
                .ToList();

        var rows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.AccountCode.StartsWith(
                            CustomerRootCode))
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var rowsByUpload =
            rows
                .GroupBy(
                    x => x.FinancialUploadId)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());

        var results =
            new List<BranchCustomersDto>();

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
                out var branchRows);

            branchRows ??=
                new List<FinancialAccountRow>();

            /*
             * IMPORTANT:
             *
             * A Financial Position file can contain
             * other branch customer accounts too.
             *
             * Example in the Association file:
             *
             * 123002 = Customers - Association
             * 123020 = Customers - Ramadi
             *
             * Therefore we MUST match the parent
             * with Upload.BranchId / branch name.
             */
            var customerParent =
                FindCustomerParent(
                    branchRows,
                    branch.BranchNameArabic);

            if (customerParent is null)
            {
                results.Add(
                    EmptyBranch(
                        branch.Id,
                        branch.BranchCode,
                        branch.BranchNameArabic,
                        true));

                continue;
            }

            var employeeParent =
                FindEmployeeParent(
                    branchRows,
                    customerParent,
                    branch.BranchNameArabic);

            results.Add(
                new BranchCustomersDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasFinancialPosition =
                        true,

                    HasCustomerAccount =
                        true,

                    HasEmployeeAccount =
                        employeeParent is not null,

                    CustomerAccountCode =
                        customerParent.AccountCode,

                    CustomerAccountName =
                        customerParent.AccountName,

                    CustomersTotal =
                        GetFinancialPositionValue(
                            customerParent),

                    EmployeeAccountCode =
                        employeeParent?.AccountCode,

                    EmployeeAccountName =
                        employeeParent?.AccountName,

                    EmployeesTotal =
                        employeeParent is null
                            ? 0
                            : GetFinancialPositionValue(
                                employeeParent)
                });
        }

        var customersWithData =
            results
                .Where(
                    x => x.HasCustomerAccount)
                .ToList();

        var employeesWithData =
            results
                .Where(
                    x => x.HasEmployeeAccount)
                .ToList();

        var customersTotal =
            customersWithData.Sum(
                x => x.CustomersTotal);

        var employeesTotal =
            employeesWithData.Sum(
                x => x.EmployeesTotal);

        return new CustomersAnalysisDto
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

            BranchesWithCustomerData =
                customersWithData.Count,

            BranchesWithoutCustomerData =
                branches.Count -
                customersWithData.Count,

            BranchesWithEmployeeData =
                employeesWithData.Count,

            AllBranchesCustomersTotal =
                customersTotal,

            AllBranchesEmployeesTotal =
                employeesTotal,

            AverageCustomers =
                customersWithData.Count == 0
                    ? 0
                    : Math.Round(
                        customersTotal /
                        customersWithData.Count,
                        2),

            AverageEmployees =
                employeesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        employeesTotal /
                        employeesWithData.Count,
                        2),

            HighestCustomersBranch =
                ToRanking(
                    customersWithData
                        .OrderByDescending(
                            x => x.CustomersTotal)
                        .ThenBy(
                            x => x.BranchCode)
                        .FirstOrDefault(),
                    x => x.CustomersTotal),

            LowestCustomersBranch =
                ToRanking(
                    customersWithData
                        .OrderBy(
                            x => x.CustomersTotal)
                        .ThenBy(
                            x => x.BranchCode)
                        .FirstOrDefault(),
                    x => x.CustomersTotal),

            HighestEmployeesBranch =
                ToRanking(
                    employeesWithData
                        .OrderByDescending(
                            x => x.EmployeesTotal)
                        .ThenBy(
                            x => x.BranchCode)
                        .FirstOrDefault(),
                    x => x.EmployeesTotal),

            LowestEmployeesBranch =
                ToRanking(
                    employeesWithData
                        .OrderBy(
                            x => x.EmployeesTotal)
                        .ThenBy(
                            x => x.BranchCode)
                        .FirstOrDefault(),
                    x => x.EmployeesTotal),

            Branches =
                results
        };
    }

    private static FinancialAccountRow? FindCustomerParent(
        IReadOnlyCollection<FinancialAccountRow> rows,
        string branchName)
    {
        var branchToken =
            NormalizeBranchName(
                branchName);

        return rows
            .Where(
                x =>
                    x.AccountCode !=
                        CustomerRootCode &&
                    x.AccountCode.StartsWith(
                        CustomerRootCode,
                        StringComparison.Ordinal) &&
                    ContainsCustomerWord(
                        x.AccountName) &&
                    ContainsBranchName(
                        x.AccountName,
                        branchToken) &&
                    !ContainsEmployeeWord(
                        x.AccountName))
            .OrderBy(
                x => x.AccountCode.Length)
            .ThenBy(
                x => x.RowOrder)
            .FirstOrDefault();
    }

    private static FinancialAccountRow? FindEmployeeParent(
        IReadOnlyCollection<FinancialAccountRow> rows,
        FinancialAccountRow customerParent,
        string branchName)
    {
        var branchToken =
            NormalizeBranchName(
                branchName);

        return rows
            .Where(
                x =>
                    x.AccountCode !=
                        customerParent.AccountCode &&
                    x.AccountCode.StartsWith(
                        customerParent.AccountCode,
                        StringComparison.Ordinal) &&
                    ContainsEmployeeWord(
                        x.AccountName) &&
                    ContainsBranchName(
                        x.AccountName,
                        branchToken))
            .OrderBy(
                x => x.AccountCode.Length)
            .ThenBy(
                x => x.RowOrder)
            .FirstOrDefault();
    }

    private static bool ContainsCustomerWord(
        string value)
    {
        var normalized =
            NormalizeArabic(value);

        return normalized.Contains(
            NormalizeArabic("زبائن"),
            StringComparison.Ordinal);
    }

    private static bool ContainsEmployeeWord(
        string value)
    {
        var normalized =
            NormalizeArabic(value);

        return
            normalized.Contains(
                NormalizeArabic("موظف"),
                StringComparison.Ordinal) ||
            normalized.Contains(
                NormalizeArabic("موظفي"),
                StringComparison.Ordinal) ||
            normalized.Contains(
                NormalizeArabic("موظفة"),
                StringComparison.Ordinal);
    }

    private static bool ContainsBranchName(
        string accountName,
        string branchToken)
    {
        if (string.IsNullOrWhiteSpace(
                branchToken))
        {
            return false;
        }

        var normalized =
            NormalizeBranchName(
                accountName);

        return normalized.Contains(
            branchToken,
            StringComparison.Ordinal);
    }

    private static string NormalizeBranchName(
        string value)
    {
        var normalized =
            NormalizeArabic(value);

        var wordsToRemove =
            new[]
            {
                "فرع",
                "زبائن",
                "موظفي",
                "موظف",
                "موظفة"
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
                builder.Append(character);
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

    private static decimal GetFinancialPositionValue(
        FinancialAccountRow row)
    {
        /*
         * Financial Position source:
         *
         * Assets / Liabilities
         *
         * Return a signed financial value:
         *
         * Assets      => positive
         * Liabilities => negative
         *
         * Customers and employees are normally
         * Assets, but this keeps the result correct
         * if the source accounting value changes side.
         */

        var assets =
            row.Assets ?? 0;

        var liabilities =
            row.Liabilities ?? 0;

        return assets - liabilities;
    }

    private static BranchCustomersDto EmptyBranch(
        int branchId,
        string branchCode,
        string branchName,
        bool hasFinancialPosition)
    {
        return new BranchCustomersDto
        {
            BranchId =
                branchId,

            BranchCode =
                branchCode,

            BranchName =
                branchName,

            HasFinancialPosition =
                hasFinancialPosition,

            HasCustomerAccount =
                false,

            HasEmployeeAccount =
                false,

            CustomerAccountCode =
                null,

            CustomerAccountName =
                null,

            CustomersTotal =
                0,

            EmployeeAccountCode =
                null,

            EmployeeAccountName =
                null,

            EmployeesTotal =
                0
        };
    }

    private static CustomerBranchRankingDto? ToRanking(
        BranchCustomersDto? branch,
        Func<BranchCustomersDto,decimal> selector)
    {
        if (branch is null)
            return null;

        return new CustomerBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            Value =
                selector(branch)
        };
    }
}
