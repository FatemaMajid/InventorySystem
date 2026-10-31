using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Common.Excel;

public class FinancialExcelParser
{
    private readonly IApplicationDbContext _context;

    public FinancialExcelParser(IApplicationDbContext context) {
        _context = context;
    }

    public async Task<FinancialExcelParseResult> ParseAsync(
        FinancialUpload upload,
        byte[] content,
        CancellationToken cancellationToken)
    {
        if (content.Length == 0)
        {
            throw new InvalidDataException(
                "The Excel file is empty.");
        }

        try
        {
            using var stream =
                new MemoryStream(content);

            using var workbook =
                new XLWorkbook(stream);

            var worksheet =
                workbook.Worksheets.FirstOrDefault();

            if (worksheet is null){
                throw new InvalidDataException(
                    "The Excel file does not contain any worksheets.");
            }

            return upload.FileType switch
            {
                FinancialFileType.IncomeStatement =>
                    ParseIncomeStatement(
                        worksheet,
                        upload.Id),

                FinancialFileType.FinancialPosition =>
                    ParseFinancialPosition(
                        worksheet,
                        upload.Id),

                FinancialFileType.Transfer =>
                    await ParseTransferAsync(
                        worksheet,
                        upload,
                        cancellationToken),

                FinancialFileType.Settlement =>
                    await ParseSettlementAsync(
                        worksheet,
                        upload,
                        cancellationToken),

                _ =>
                    throw new InvalidDataException(
                        "Unsupported financial file type.")
            };
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(
                $"The Excel file could not be parsed: {ex.Message}",
                ex);
        }
    }

    private static FinancialExcelParseResult
        ParseIncomeStatement(
            IXLWorksheet worksheet,
            int uploadId)
    {
        var headerRow =
            FindHeaderRow(
                worksheet,
                "رقم الحساب",
                "الاسم",
                "مدين",
                "دائن",
                "رصيد");

        if (headerRow == 0)
        {
            throw new InvalidDataException(
                "Income statement headers were not found. Expected: رقم الحساب، الاسم، مدين، دائن، رصيد.");
        }

        var accountCodeColumn =
            FindColumn(
                worksheet,
                headerRow,
                "رقم الحساب");

        var accountNameColumn =
            FindColumn(
                worksheet,
                headerRow,
                "الاسم",
                "اسم الحساب");

        var debitColumn =
            FindColumn(
                worksheet,
                headerRow,
                "مدين");

        var creditColumn =
            FindColumn(
                worksheet,
                headerRow,
                "دائن");

        var balanceColumn =
            FindColumn(
                worksheet,
                headerRow,
                "رصيد");

        var lastRow =
            worksheet.LastRowUsed()?.RowNumber()
            ?? headerRow;

        var result =
            new FinancialExcelParseResult();

        var rowOrder = 1;

        for (var rowNumber = headerRow + 1;
             rowNumber <= lastRow;
             rowNumber++)
        {
            var row =
                worksheet.Row(rowNumber);

            var accountCode =
                CleanAccountCode(
                    GetText(
                        row.Cell(
                            accountCodeColumn)));

            var accountName =
                CleanText(
                    GetText(
                        row.Cell(
                            accountNameColumn)));

            if (!IsNumericAccountCode(
                    accountCode))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(
                    accountName))
            {
                throw new InvalidDataException(
                    $"Income statement row {rowNumber} has an account code but no account name.");
            }

            result.AccountRows.Add(
                new FinancialAccountRow
                {
                    FinancialUploadId =
                        uploadId,

                    AccountCode =
                        accountCode,

                    AccountName =
                        accountName,

                    Debit =
                        GetNullableDecimal(
                            row.Cell(
                                debitColumn),
                            rowNumber,
                            "مدين"),

                    Credit =
                        GetNullableDecimal(
                            row.Cell(
                                creditColumn),
                            rowNumber,
                            "دائن"),

                    Balance =
                        GetNullableDecimal(
                            row.Cell(
                                balanceColumn),
                            rowNumber,
                            "رصيد"),

                    Assets =
                        null,

                    Liabilities =
                        null,

                    RowOrder =
                        rowOrder++
                });
        }

        if (result.AccountRows.Count == 0)
        {
            throw new InvalidDataException(
                "The income statement does not contain any valid account rows.");
        }

        return result;
    }

    private static FinancialExcelParseResult
        ParseFinancialPosition(
            IXLWorksheet worksheet,
            int uploadId)
    {
        var headerRow =
            FindHeaderRow(
                worksheet,
                "رقم الحساب",
                "اسم الحساب",
                "الأصول",
                "الخصوم");

        if (headerRow == 0)
        {
            throw new InvalidDataException(
                "Financial position headers were not found. Expected: رقم الحساب، اسم الحساب، الأصول، الخصوم.");
        }

        var accountCodeColumn =
            FindColumn(
                worksheet,
                headerRow,
                "رقم الحساب");

        var accountNameColumn =
            FindColumn(
                worksheet,
                headerRow,
                "اسم الحساب",
                "الاسم");

        var assetsColumn =
            FindColumn(
                worksheet,
                headerRow,
                "الأصول",
                "الاصول");

        var liabilitiesColumn =
            FindColumn(
                worksheet,
                headerRow,
                "الخصوم");

        var lastRow =
            worksheet.LastRowUsed()?.RowNumber()
            ?? headerRow;

        var result =
            new FinancialExcelParseResult();

        var rowOrder = 1;

        for (var rowNumber = headerRow + 1;
             rowNumber <= lastRow;
             rowNumber++)
        {
            var row =
                worksheet.Row(rowNumber);

            var accountCode =
                CleanAccountCode(
                    GetText(
                        row.Cell(
                            accountCodeColumn)));

            var accountName =
                CleanText(
                    GetText(
                        row.Cell(
                            accountNameColumn)));

            if (!IsNumericAccountCode(
                    accountCode))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(
                    accountName))
            {
                throw new InvalidDataException(
                    $"Financial position row {rowNumber} has an account code but no account name.");
            }

            result.AccountRows.Add(
                new FinancialAccountRow
                {
                    FinancialUploadId =
                        uploadId,

                    AccountCode =
                        accountCode,

                    AccountName =
                        accountName,

                    Debit =
                        null,

                    Credit =
                        null,

                    Balance =
                        null,

                    Assets =
                        GetNullableDecimal(
                            row.Cell(
                                assetsColumn),
                            rowNumber,
                            "الأصول"),

                    Liabilities =
                        GetNullableDecimal(
                            row.Cell(
                                liabilitiesColumn),
                            rowNumber,
                            "الخصوم"),

                    RowOrder =
                        rowOrder++
                });
        }

        if (result.AccountRows.Count == 0)
        {
            throw new InvalidDataException(
                "The financial position file does not contain any valid account rows.");
        }

        return result;
    }

    private async Task<FinancialExcelParseResult>
        ParseTransferAsync(
            IXLWorksheet worksheet,
            FinancialUpload upload,
            CancellationToken cancellationToken)
    {
        if (!upload.Gender.HasValue)
        {
            throw new InvalidDataException(
                "Transfer file gender is required.");
        }

        var headerRow =
            FindHeaderRow(
                worksheet,
                "رقم الحساب",
                "الاسم");

        if (headerRow == 0)
        {
            throw new InvalidDataException(
                "Transfer file headers were not found.");
        }

        var accountCodeColumn =
            FindColumn(
                worksheet,
                headerRow,
                "رقم الحساب");

        var accountNameColumn =
            FindColumn(
                worksheet,
                headerRow,
                "الاسم",
                "الاســــــــم");

        var balanceHeaderRow =
            headerRow + 1;

        var debitColumn =
            FindColumn(
                worksheet,
                balanceHeaderRow,
                "مدين");

        var creditColumn =
            FindColumn(
                worksheet,
                balanceHeaderRow,
                "دائن");

        var financialBranches =
            await GetFinancialBranchesAsync(
                cancellationToken);

        var lastRow =
            worksheet.LastRowUsed()?.RowNumber()
            ?? balanceHeaderRow;

        var result =
            new FinancialExcelParseResult();

        var rowOrder = 1;

        for (var rowNumber = balanceHeaderRow + 1;
             rowNumber <= lastRow;
             rowNumber++)
        {
            var row =
                worksheet.Row(rowNumber);

            var accountCode =
                CleanAccountCode(
                    GetText(
                        row.Cell(
                            accountCodeColumn)));

            var accountName =
                CleanText(
                    GetText(
                        row.Cell(
                            accountNameColumn)));

            if (string.IsNullOrWhiteSpace(
                    accountName))
            {
                continue;
            }

            var normalizedName =
                NormalizeArabic(
                    accountName);

            if (normalizedName.Contains(
                    "المجموع"))
            {
                continue;
            }

            if (normalizedName.StartsWith(
                    "فروع "))
            {
                continue;
            }

            if (!IsNumericAccountCode(
                    accountCode))
            {
                continue;
            }

            var branch =
                ResolveBranch(
                    financialBranches,
                    accountName);

            if (branch is null)
            {
                if (normalizedName.Contains(
                        "فرع"))
                {
                    throw new InvalidDataException(
                        $"Could not match transfer row {rowNumber} branch '{accountName}' with an active system branch.");
                }

                continue;
            }

            var debit =
                GetNullableDecimal(
                    row.Cell(
                        debitColumn),
                    rowNumber,
                    "مدين")
                ?? 0m;

            var credit =
                GetNullableDecimal(
                    row.Cell(
                        creditColumn),
                    rowNumber,
                    "دائن")
                ?? 0m;

            result.TransferRecords.Add(
    new TransferRecord
    {
        FinancialUploadId =
            upload.Id,

        BranchId = branch.Id,

        AccountCode =
            accountCode,

        AccountName =
            accountName,

        Gender =
            upload.Gender.Value,

        Value =
            debit - credit,

        RowOrder =
            rowOrder++
    });
        }

        if (result.TransferRecords.Count == 0)
        {
            throw new InvalidDataException(
                "The transfer file does not contain any branch records.");
        }

        var duplicateBranches =
            result.TransferRecords
                .GroupBy(x => x.BranchId)
                .Where(x => x.Count() > 1)
                .ToList();

        if (duplicateBranches.Count > 0)
        {
            throw new InvalidDataException(
                "The transfer file contains duplicate branch records.");
        }

        return result;
    }

    private async Task<FinancialExcelParseResult>
        ParseSettlementAsync(
            IXLWorksheet worksheet,
            FinancialUpload upload,
            CancellationToken cancellationToken)
    {
        if (!upload.BranchId.HasValue)
        {
            throw new InvalidDataException(
                "Settlement branch is required.");
        }

        if (!upload.Gender.HasValue)
        {
            throw new InvalidDataException(
                "Settlement gender is required.");
        }

        var branch =
            await _context.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                            upload.BranchId.Value &&
                        !x.IsDeleted &&
                        x.IsActive,
                    cancellationToken);

        if (branch is null)
        {
            throw new InvalidDataException(
                "The settlement branch was not found or is inactive.");
        }

        var metadataHeaderRow =
            FindHeaderRow(
                worksheet,
                "رقم الحساب",
                "اسم الحساب");

        if (metadataHeaderRow == 0)
        {
            throw new InvalidDataException(
                "Settlement account information was not found.");
        }

        var accountNameColumn =
            FindColumn(
                worksheet,
                metadataHeaderRow,
                "اسم الحساب");

        var accountDataRow =
            metadataHeaderRow + 1;

        var accountName =
            CleanText(
                GetText(
                    worksheet.Row(
                        accountDataRow)
                    .Cell(
                        accountNameColumn)));

        if (string.IsNullOrWhiteSpace(
                accountName))
        {
            throw new InvalidDataException(
                "Settlement account name is missing.");
        }

        ValidateSettlementBranch(
            accountName,
            branch);

        ValidateSettlementGender(
            accountName,
            upload.Gender.Value);

        var transactionHeaderRow =
            FindHeaderRow(
                worksheet,
                "التاريخ",
                "البيان",
                "رقم القيد",
                "مدين",
                "دائن",
                "الرصيد");

        if (transactionHeaderRow == 0)
        {
            throw new InvalidDataException(
                "Settlement transaction headers were not found.");
        }

        var dateColumn =
            FindColumn(
                worksheet,
                transactionHeaderRow,
                "التاريخ");

        var descriptionColumn =
            FindColumn(
                worksheet,
                transactionHeaderRow,
                "البيان");

        var entryNumberColumn =
            FindColumn(
                worksheet,
                transactionHeaderRow,
                "رقم القيد");

        var debitColumn =
            FindColumn(
                worksheet,
                transactionHeaderRow,
                "مدين");

        var creditColumn =
            FindColumn(
                worksheet,
                transactionHeaderRow,
                "دائن");

        var balanceColumn =
            FindColumn(
                worksheet,
                transactionHeaderRow,
                "الرصيد");

        var markedColumn =
            FindOptionalColumn(
                worksheet,
                transactionHeaderRow,
                "إشارة تعليم",
                "اشارة تعليم");

        var matchedColumn =
            FindOptionalColumn(
                worksheet,
                transactionHeaderRow,
                "إشارة مطابقة",
                "اشارة مطابقة");

        var result =
            new FinancialExcelParseResult();

        var lastRow =
            worksheet.LastRowUsed()?.RowNumber()
            ?? transactionHeaderRow;

        var rowOrder = 1;
        var summaryFound = false;

        for (var rowNumber =
                 transactionHeaderRow + 1;
             rowNumber <= lastRow;
             rowNumber++)
        {
            var row =
                worksheet.Row(rowNumber);

            var description =
                CleanText(
                    GetText(
                        row.Cell(
                            descriptionColumn)));

            if (string.IsNullOrWhiteSpace(
                    description))
            {
                continue;
            }

            var normalizedDescription =
                NormalizeArabic(
                    description);

            if (normalizedDescription.Contains(
                    "مجموع الفتره"))
            {
                result.SettlementSummary =
                    new SettlementSummary
                    {
                        FinancialUploadId =
                            upload.Id,

                        PeriodDebit =
                            GetNullableDecimal(
                                row.Cell(
                                    debitColumn),
                                rowNumber,
                                "مدين")
                            ?? 0m,

                        PeriodCredit =
                            GetNullableDecimal(
                                row.Cell(
                                    creditColumn),
                                rowNumber,
                                "دائن")
                            ?? 0m,

                        PeriodBalance =
                            GetNullableDecimal(
                                row.Cell(
                                    balanceColumn),
                                rowNumber,
                                "الرصيد")
                            ?? 0m
                    };

                summaryFound = true;
                continue;
            }

            if (IsSettlementSummaryRow(
                    normalizedDescription))
            {
                continue;
            }

            var transactionDate =
                GetNullableDate(
                    row.Cell(
                        dateColumn),
                    rowNumber);

            var entryNumber =
                CleanText(
                    GetText(
                        row.Cell(
                            entryNumberColumn)));

            var debit =
                GetNullableDecimal(
                    row.Cell(
                        debitColumn),
                    rowNumber,
                    "مدين");

            var credit =
                GetNullableDecimal(
                    row.Cell(
                        creditColumn),
                    rowNumber,
                    "دائن");

            var balance =
                GetNullableDecimal(
                    row.Cell(
                        balanceColumn),
                    rowNumber,
                    "الرصيد");

            if (!transactionDate.HasValue &&
                string.IsNullOrWhiteSpace(
                    entryNumber) &&
                !debit.HasValue &&
                !credit.HasValue &&
                !balance.HasValue)
            {
                continue;
            }

            result.SettlementRows.Add(
                new SettlementRow
                {
                    FinancialUploadId =
                        upload.Id,

                    TransactionDate =
                        transactionDate,

                    Description =
                        description,

                    EntryNumber =
                        string.IsNullOrWhiteSpace(
                            entryNumber)
                            ? null
                            : entryNumber,

                    Debit =
                        debit,

                    Credit =
                        credit,

                    Balance =
                        balance,

                    IsMarked =
                        markedColumn > 0 &&
                        GetBoolean(
                            row.Cell(
                                markedColumn)),

                    IsMatched =
                        matchedColumn > 0 &&
                        GetBoolean(
                            row.Cell(
                                matchedColumn)),

                    RowOrder =
                        rowOrder++
                });
        }

        if (result.SettlementRows.Count == 0)
        {
            throw new InvalidDataException(
                "The settlement file does not contain any transactions.");
        }

        if (!summaryFound ||
            result.SettlementSummary is null)
        {
            throw new InvalidDataException(
                "The settlement file does not contain a 'مجموع الفترة' summary row.");
        }

        return result;
    }

    private async Task<List<Branch>>
        GetFinancialBranchesAsync(
            CancellationToken cancellationToken)
    {
        return await _context.Branches
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
            .ToListAsync(
                cancellationToken);
    }

    private static Branch? ResolveBranch(
        IEnumerable<Branch> branches,
        string excelBranchName)
    {
        var excelName =
            NormalizeBranchName(
                excelBranchName);

        if (string.IsNullOrWhiteSpace(
                excelName))
        {
            return null;
        }

        var exact =
            branches.FirstOrDefault(
                x =>
                    NormalizeBranchName(
                        x.BranchNameArabic) ==
                    excelName);

        if (exact is not null)
        {
            return exact;
        }

        return branches
            .Select(
                x =>
                    new
                    {
                        Branch = x,
                        Name =
                            NormalizeBranchName(
                                x.BranchNameArabic)
                    })
            .Where(
                x =>
                    !string.IsNullOrWhiteSpace(
                        x.Name) &&
                    (excelName.Contains(
                         x.Name) ||
                     x.Name.Contains(
                         excelName)))
            .OrderByDescending(
                x => x.Name.Length)
            .Select(
                x => x.Branch)
            .FirstOrDefault();
    }

    private static void ValidateSettlementBranch(
        string accountName,
        Branch branch)
    {
        var excelBranchName =
            NormalizeBranchName(
                accountName);

        var systemBranchName =
            NormalizeBranchName(
                branch.BranchNameArabic);

        if (string.IsNullOrWhiteSpace(
                excelBranchName) ||
            string.IsNullOrWhiteSpace(
                systemBranchName))
        {
            return;
        }

        if (!excelBranchName.Contains(
                systemBranchName) &&
            !systemBranchName.Contains(
                excelBranchName))
        {
            throw new InvalidDataException(
                $"Settlement account '{accountName}' does not match selected branch '{branch.BranchNameArabic}'.");
        }
    }

    private static void ValidateSettlementGender(
    string accountName,
    FinancialGender gender)
    {
        var normalized =
            NormalizeArabic(
                accountName);

        var maleKeyword =
            NormalizeArabic(
                "رجالي");

        var femaleKeyword =
            NormalizeArabic(
                "نسائي");

        if (gender ==
                FinancialGender.Male &&
            !normalized.Contains(
                maleKeyword))
        {
            throw new InvalidDataException(
                "The settlement file is not a male account file.");
        }

        if (gender ==
                FinancialGender.Female &&
            !normalized.Contains(
                femaleKeyword))
        {
            throw new InvalidDataException(
                "The settlement file is not a female account file.");
        }
    }

    private static int FindHeaderRow(
        IXLWorksheet worksheet,
        params string[] requiredHeaders)
    {
        var lastRow =
            Math.Min(
                worksheet.LastRowUsed()
                    ?.RowNumber()
                ?? 0,
                20);

        var lastColumn =
            Math.Min(
                worksheet.LastColumnUsed()
                    ?.ColumnNumber()
                ?? 0,
                20);

        for (var rowNumber = 1;
             rowNumber <= lastRow;
             rowNumber++)
        {
            var values =
                new List<string>();

            for (var column = 1;
                 column <= lastColumn;
                 column++)
            {
                values.Add(
                    NormalizeHeader(
                        GetText(
                            worksheet.Cell(
                                rowNumber,
                                column))));
            }

            var foundAll =
                requiredHeaders.All(
                    required =>
                        values.Any(
                            value =>
                                HeaderMatches(
                                    value,
                                    required)));

            if (foundAll)
            {
                return rowNumber;
            }
        }

        return 0;
    }

    private static int FindColumn(
        IXLWorksheet worksheet,
        int rowNumber,
        params string[] headers)
    {
        var column =
            FindOptionalColumn(
                worksheet,
                rowNumber,
                headers);

        if (column == 0)
        {
            throw new InvalidDataException(
                $"Required Excel column '{string.Join(" / ", headers)}' was not found.");
        }

        return column;
    }

    private static int FindOptionalColumn(
        IXLWorksheet worksheet,
        int rowNumber,
        params string[] headers)
    {
        var lastColumn =
            worksheet.LastColumnUsed()
                ?.ColumnNumber()
            ?? 0;

        for (var column = 1;
             column <= lastColumn;
             column++)
        {
            var currentHeader =
                NormalizeHeader(
                    GetText(
                        worksheet.Cell(
                            rowNumber,
                            column)));

            if (headers.Any(
                    header =>
                        HeaderMatches(
                            currentHeader,
                            header)))
            {
                return column;
            }
        }

        return 0;
    }

    private static bool HeaderMatches(
    string actual,
    string expected)
    {
        if (string.IsNullOrWhiteSpace(actual) ||
            string.IsNullOrWhiteSpace(expected))
        {
            return false;
        }

        var normalizedExpected =
            NormalizeHeader(expected);

        if (string.IsNullOrWhiteSpace(
                normalizedExpected))
        {
            return false;
        }

        return actual ==
               normalizedExpected ||
               actual.Contains(
                   normalizedExpected) ||
               normalizedExpected.Contains(
                   actual);
    }

    private static string GetText(
        IXLCell cell)
    {
        return CleanText(
            cell.GetFormattedString());
    }

    private static decimal? GetNullableDecimal(
        IXLCell cell,
        int rowNumber,
        string columnName)
    {
        if (cell.IsEmpty())
        {
            return null;
        }

        if (cell.TryGetValue<decimal>(
                out var decimalValue))
        {
            return decimalValue;
        }

        var text =
            GetText(cell);

        if (string.IsNullOrWhiteSpace(
                text))
        {
            return null;
        }

        if (TryParseFinancialDecimal(
                text,
                out var value))
        {
            return value;
        }

        throw new InvalidDataException(
            $"Invalid numeric value in row {rowNumber}, column '{columnName}': '{text}'.");
    }

    private static bool TryParseFinancialDecimal(
        string input,
        out decimal value)
    {
        value = 0m;

        if (string.IsNullOrWhiteSpace(
                input))
        {
            return false;
        }

        var text =
            CleanText(input);

        text =
            text.Replace(
                    "د.ع",
                    "",
                    StringComparison.OrdinalIgnoreCase)
                .Replace(
                    "د ع",
                    "",
                    StringComparison.OrdinalIgnoreCase)
                .Replace(
                    "دينار عراقي",
                    "",
                    StringComparison.OrdinalIgnoreCase)
                .Replace(
                    ",",
                    "")
                .Replace(
                    "٬",
                    "")
                .Trim();

        var negative = false;

        if (text.EndsWith(
                "-",
                StringComparison.Ordinal))
        {
            negative = true;
            text =
                text[..^1]
                    .Trim();
        }

        if (text.StartsWith(
                "(",
                StringComparison.Ordinal) &&
            text.EndsWith(
                ")",
                StringComparison.Ordinal))
        {
            negative = true;
            text =
                text[1..^1]
                    .Trim();
        }

        if (!decimal.TryParse(
                text,
                NumberStyles.Number |
                NumberStyles.AllowLeadingSign |
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out value))
        {
            return false;
        }

        if (negative &&
            value > 0)
        {
            value *= -1m;
        }

        return true;
    }

    private static DateTime? GetNullableDate(
        IXLCell cell,
        int rowNumber)
    {
        if (cell.IsEmpty())
        {
            return null;
        }

        if (cell.TryGetValue<DateTime>(
                out var dateValue))
        {
            return dateValue;
        }

        if (cell.TryGetValue<double>(
                out var serialValue))
        {
            try
            {
                return DateTime.FromOADate(
                    serialValue);
            }
            catch
            {
                throw new InvalidDataException(
                    $"Invalid Excel date in row {rowNumber}.");
            }
        }

        var text =
            GetText(cell);

        if (DateTime.TryParse(
                text,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            return parsed;
        }

        if (DateTime.TryParse(
                text,
                new CultureInfo("ar-IQ"),
                DateTimeStyles.None,
                out parsed))
        {
            return parsed;
        }

        throw new InvalidDataException(
            $"Invalid transaction date in row {rowNumber}: '{text}'.");
    }

    private static bool GetBoolean(
        IXLCell cell)
    {
        if (cell.IsEmpty())
        {
            return false;
        }

        if (cell.TryGetValue<bool>(
                out var boolValue))
        {
            return boolValue;
        }

        var text =
            NormalizeArabic(
                GetText(cell));

        return text is
            "true" or
            "1" or
            "نعم" or
            "صح";
    }

    private static bool IsSettlementSummaryRow(
        string normalizedDescription)
    {
        return
            normalizedDescription.Contains(
                "الرصيد السابق") ||
            normalizedDescription.StartsWith(
                "رصيد ") ||
            normalizedDescription.Contains(
                "الرصيد النهائي");
    }

    private static bool IsNumericAccountCode(
        string value)
    {
        return
            !string.IsNullOrWhiteSpace(
                value) &&
            value.All(
                char.IsDigit);
    }

    private static string CleanAccountCode(
    string value)
    {
        var cleaned =
            CleanText(value)
                .Replace("٠", "0")
                .Replace("١", "1")
                .Replace("٢", "2")
                .Replace("٣", "3")
                .Replace("٤", "4")
                .Replace("٥", "5")
                .Replace("٦", "6")
                .Replace("٧", "7")
                .Replace("٨", "8")
                .Replace("٩", "9");

        return new string(
            cleaned
                .Where(char.IsDigit)
                .ToArray());
    }

    private static string NormalizeHeader(
        string value)
    {
        return NormalizeArabic(
            value)
            .Replace(
                " ",
                "");
    }

    private static string NormalizeBranchName(
    string value)
    {
        var normalized =
            NormalizeArabic(value);

        var wordsToRemove =
            new[]
            {
            "الرجالي",
            "رجالي",
            "النسائي",
            "نسائي",
            "والاطفال",
            "الاطفال",
            "اطفال"
            };

        foreach (var word in wordsToRemove)
        {
            normalized =
                normalized.Replace(
                    NormalizeArabic(word),
                    " ",
                    StringComparison.Ordinal);
        }

        var parts =
            normalized
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(
                    x => x != NormalizeArabic("فرع"))
                .ToArray();

        return string.Join(
            ' ',
            parts);
    }

    private static string NormalizeArabic(
        string value)
    {
        var cleaned = CleanText(value)
                .ToLowerInvariant()
                .Replace('أ', 'ا')
                .Replace('إ', 'ا')
                .Replace('آ', 'ا')
                .Replace('ى', 'ي')
                .Replace('ة', 'ه')
                .Replace('ؤ', 'و')
                .Replace('ئ', 'ي')
                .Replace("ـ", "")
                .Replace('-', ' ')
                .Replace('_', ' ')
                .Replace('/', ' ');

        var builder =
            new StringBuilder();

        foreach (var character in
                 cleaned)
        {
            var category =
                CharUnicodeInfo
                    .GetUnicodeCategory(
                        character);

            if (category ==
                    UnicodeCategory.NonSpacingMark ||
                category ==
                    UnicodeCategory.SpacingCombiningMark ||
                category ==
                    UnicodeCategory.EnclosingMark)
            {
                continue;
            }

            builder.Append(
                character);
        }

        return string.Join(
            ' ',
            builder
                .ToString()
                .Split(
                    ' ',
                    StringSplitOptions
                        .RemoveEmptyEntries));
    }

    private static string CleanText(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(
                value))
        {
            return string.Empty;
        }

        var builder =
            new StringBuilder();

        foreach (var character in value)
        {
            var category =
                CharUnicodeInfo
                    .GetUnicodeCategory(
                        character);

            if (category ==
                    UnicodeCategory.Format ||
                category ==
                    UnicodeCategory.Control)
            {
                if (character == '\t' ||
                    character == '\r' ||
                    character == '\n')
                {
                    builder.Append(' ');
                }

                continue;
            }

            builder.Append(
                character);
        }

        return string.Join(
            ' ',
            builder
                .ToString()
                .Trim()
                .Split(
                    ' ',
                    StringSplitOptions
                        .RemoveEmptyEntries));
    }
}
