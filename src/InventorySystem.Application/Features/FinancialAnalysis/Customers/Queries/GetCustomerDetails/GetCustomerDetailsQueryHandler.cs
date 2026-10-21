using System.Globalization;
using System.Text;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Customers;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Customers.Queries.GetCustomerDetails;

public sealed class GetCustomerDetailsQueryHandler
    : IRequestHandler<
        GetCustomerDetailsQuery,
        CustomerEmployeeDetailsDto?>
{
    private const string CustomerRootCode =
        "123";

    private readonly IApplicationDbContext _context;

    public GetCustomerDetailsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerEmployeeDetailsDto?> Handle(
        GetCustomerDetailsQuery request,
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
                            CustomerRootCode))
                .OrderBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var branchToken =
            NormalizeBranchName(
                branch.BranchNameArabic);

        var customerParent =
            rows
                .Where(
                    x =>
                        x.AccountCode !=
                            CustomerRootCode &&
                        x.AccountCode.StartsWith(
                            CustomerRootCode,
                            StringComparison.Ordinal) &&
                        ContainsCustomerWord(
                            x.AccountName) &&
                        !ContainsEmployeeWord(
                            x.AccountName) &&
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

        if (customerParent is null)
            return null;

        var employeeParent =
            rows
                .Where(
                    x =>
                        x.AccountCode !=
                            customerParent.AccountCode &&
                        x.AccountCode.StartsWith(
                            customerParent.AccountCode,
                            StringComparison.Ordinal) &&
                        ContainsEmployeeWord(
                            x.AccountName) &&
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

        var employeeDetails =
            employeeParent is null
                ? new List<CustomerDetailRowDto>()
                : rows
                    .Where(
                        x =>
                            x.AccountCode !=
                                employeeParent.AccountCode &&
                            x.AccountCode.StartsWith(
                                employeeParent.AccountCode,
                                StringComparison.Ordinal))
                    .OrderBy(
                        x => x.RowOrder)
                    .Select(
                        ToDetail)
                    .ToList();

        /*
         * Customer details exclude the employee
         * subtree because employees have their own
         * section/details.
         *
         * CustomerTotal remains the CUSTOMER PARENT
         * value exactly as required.
         */
        var customerDetails =
            rows
                .Where(
                    x =>
                        x.AccountCode !=
                            customerParent.AccountCode &&
                        x.AccountCode.StartsWith(
                            customerParent.AccountCode,
                            StringComparison.Ordinal) &&
                        (
                            employeeParent == null ||
                            !x.AccountCode.StartsWith(
                                employeeParent.AccountCode,
                                StringComparison.Ordinal)
                        ))
                .OrderBy(
                    x => x.RowOrder)
                .Select(
                    ToDetail)
                .ToList();

        return new CustomerEmployeeDetailsDto
        {
            FinancialPeriodId =
                request.FinancialPeriodId,

            BranchId =
                branch.Id,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchNameArabic,

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
                        employeeParent),

            CustomerDetailsCount =
                customerDetails.Count,

            EmployeeDetailsCount =
                employeeDetails.Count,

            CustomerDetails =
                customerDetails,

            EmployeeDetails =
                employeeDetails
        };
    }

    private static CustomerDetailRowDto ToDetail(
        FinancialAccountRow row)
    {
        return new CustomerDetailRowDto
        {
            AccountCode =
                row.AccountCode,

            AccountName =
                row.AccountName,

            Assets =
                row.Assets ?? 0,

            Liabilities =
                row.Liabilities ?? 0,

            Value =
                GetFinancialPositionValue(
                    row),

            RowOrder =
                row.RowOrder
        };
    }

    private static decimal GetFinancialPositionValue(
        FinancialAccountRow row)
    {
        return
            (row.Assets ?? 0) -
            (row.Liabilities ?? 0);
    }

    private static bool ContainsCustomerWord(
        string value)
    {
        return NormalizeArabic(value)
            .Contains(
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
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

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
}
