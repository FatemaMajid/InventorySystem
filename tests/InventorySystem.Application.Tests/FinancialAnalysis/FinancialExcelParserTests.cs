using ClosedXML.Excel;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Application.Features.FinancialAnalysis.Common.Excel;
using InventorySystem.Application.Features.FinancialAnalysis.Common.Exceptions;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.Tests.FinancialAnalysis;

public class FinancialExcelParserTests {
    [Fact]
    public async Task ParseAsync_EmptyFile_ThrowsExpectedValidationCode(){
        var parser = new FinancialExcelParser(null!);
        var upload = new FinancialUpload {
            FileType = FinancialFileType.IncomeStatement
        };

        var exception = await Assert.ThrowsAsync<FinancialExcelValidationException>(
            () => parser.ParseAsync(upload, Array.Empty<byte>(), CancellationToken.None));

        Assert.Equal(LocalizationKeys.Excel.EmptyFile, exception.ErrorCode);
        Assert.Null(exception.RowNumber);
    }

    [Fact]
    public async Task ParseAsync_IncomeStatementWithoutHeaders_ThrowsExpectedValidationCode(){
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Test");
        worksheet.Cell(1, 1).Value = "Unrelated Column";
        worksheet.Cell(2, 1).Value = "Sample";

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var parser = new FinancialExcelParser(null!);
        var upload = new FinancialUpload {
            FileType = FinancialFileType.IncomeStatement
        };

        var exception = await Assert.ThrowsAsync<FinancialExcelValidationException>(
            () => parser.ParseAsync(upload, stream.ToArray(), CancellationToken.None));

        Assert.Equal(LocalizationKeys.FinancialExcel.IncomeHeadersMissing, exception.ErrorCode);
        Assert.Null(exception.RowNumber);
    }

    [Fact]
    public async Task ParseAsync_ValidIncomeStatement_ReturnsCorrectAccountRows(){
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Income");
        worksheet.Cell(1, 1).Value = "رقم الحساب";
        worksheet.Cell(1, 2).Value = "الاسم";
        worksheet.Cell(1, 3).Value = "مدين";
        worksheet.Cell(1, 4).Value = "دائن";
        worksheet.Cell(1, 5).Value = "رصيد";

        worksheet.Cell(2, 1).Value = "1001";
        worksheet.Cell(2, 2).Value = "حساب النقد";
        worksheet.Cell(2, 3).Value = 125.75;
        worksheet.Cell(2, 4).Value = 25.25;
        worksheet.Cell(2, 5).Value = 100.50;

        worksheet.Cell(3, 1).Value = "1002";
        worksheet.Cell(3, 2).Value = "حساب البنك";
        worksheet.Cell(3, 3).Value = 300.25;
        worksheet.Cell(3, 4).Value = 100.00;
        worksheet.Cell(3, 5).Value = 200.25;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var parser = new FinancialExcelParser(null!);
        var upload = new FinancialUpload {
            Id = 25,
            FileType = FinancialFileType.IncomeStatement
        };

        var result = await parser.ParseAsync(upload, stream.ToArray(), CancellationToken.None);

        Assert.Equal(2, result.AccountRows.Count);

        var first = result.AccountRows[0];
        Assert.Equal(25, first.FinancialUploadId);
        Assert.Equal("1001", first.AccountCode);
        Assert.Equal("حساب النقد", first.AccountName);
        Assert.Equal(125.75m, first.Debit);
        Assert.Equal(25.25m, first.Credit);
        Assert.Equal(100.50m, first.Balance);
        Assert.Equal(1, first.RowOrder);
        Assert.Null(first.Assets);
        Assert.Null(first.Liabilities);

        var second = result.AccountRows[1];
        Assert.Equal(25, second.FinancialUploadId);
        Assert.Equal("1002", second.AccountCode);
        Assert.Equal("حساب البنك", second.AccountName);
        Assert.Equal(300.25m, second.Debit);
        Assert.Equal(100.00m, second.Credit);
        Assert.Equal(200.25m, second.Balance);
        Assert.Equal(2, second.RowOrder);
        Assert.Null(second.Assets);
        Assert.Null(second.Liabilities);
    }

    [Fact]
    public async Task ParseAsync_IncomeStatementMissingAccountName_ThrowsRowNumber(){
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Income");
        worksheet.Cell(1, 1).Value = "رقم الحساب";
        worksheet.Cell(1, 2).Value = "الاسم";
        worksheet.Cell(1, 3).Value = "مدين";
        worksheet.Cell(1, 4).Value = "دائن";
        worksheet.Cell(1, 5).Value = "رصيد";

        worksheet.Cell(2, 1).Value = "1001";
        worksheet.Cell(2, 2).Value = "";
        worksheet.Cell(2, 3).Value = 100;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var parser = new FinancialExcelParser(null!);
        var upload = new FinancialUpload {
            FileType = FinancialFileType.IncomeStatement
        };

        var exception = await Assert.ThrowsAsync<FinancialExcelValidationException>(
            () => parser.ParseAsync(upload, stream.ToArray(), CancellationToken.None));

        Assert.Equal(LocalizationKeys.FinancialExcel.AccountNameMissing, exception.ErrorCode);
        Assert.Equal(2, exception.RowNumber);
    }
    [Fact]
    public async Task ParseAsync_ValidFinancialPosition_ReturnsCorrectAccountRows(){
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Position");

        worksheet.Cell(1, 1).Value = "رقم الحساب";
        worksheet.Cell(1, 2).Value = "اسم الحساب";
        worksheet.Cell(1, 3).Value = "الأصول";
        worksheet.Cell(1, 4).Value = "الخصوم";

        worksheet.Cell(2, 1).Value = "1001";
        worksheet.Cell(2, 2).Value = "الصندوق";
        worksheet.Cell(2, 3).Value = 1250.75;
        worksheet.Cell(2, 4).Value = 250.25;

        worksheet.Cell(3, 1).Value = "2001";
        worksheet.Cell(3, 2).Value = "المصرف";
        worksheet.Cell(3, 3).Value = 3000.50;
        worksheet.Cell(3, 4).Value = 1000.25;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var parser = new FinancialExcelParser(null!);
        var upload = new FinancialUpload {
            Id = 25,
            FileType = FinancialFileType.FinancialPosition
        };

        var result = await parser.ParseAsync(upload, stream.ToArray(), CancellationToken.None);

        Assert.Equal(2, result.AccountRows.Count);

        var first = result.AccountRows[0];
        Assert.Equal(25, first.FinancialUploadId);
        Assert.Equal("1001", first.AccountCode);
        Assert.Equal("الصندوق", first.AccountName);
        Assert.Equal(1250.75m, first.Assets);
        Assert.Equal(250.25m, first.Liabilities);
        Assert.Equal(1, first.RowOrder);
        Assert.Null(first.Debit);
        Assert.Null(first.Credit);
        Assert.Null(first.Balance);

        var second = result.AccountRows[1];
        Assert.Equal(25, second.FinancialUploadId);
        Assert.Equal("2001", second.AccountCode);
        Assert.Equal("المصرف", second.AccountName);
        Assert.Equal(3000.50m, second.Assets);
        Assert.Equal(1000.25m, second.Liabilities);
        Assert.Equal(2, second.RowOrder);
        Assert.Null(second.Debit);
        Assert.Null(second.Credit);
        Assert.Null(second.Balance);
    }

    [Fact]
    public async Task ParseAsync_FinancialPositionMissingAccountName_ThrowsRowNumber(){
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Position");

        worksheet.Cell(1, 1).Value = "رقم الحساب";
        worksheet.Cell(1, 2).Value = "اسم الحساب";
        worksheet.Cell(1, 3).Value = "الأصول";
        worksheet.Cell(1, 4).Value = "الخصوم";

        worksheet.Cell(2, 1).Value = "1001";
        worksheet.Cell(2, 3).Value = 100;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var parser = new FinancialExcelParser(null!);
        var upload = new FinancialUpload {
            FileType = FinancialFileType.FinancialPosition
        };

        var exception = await Assert.ThrowsAsync<FinancialExcelValidationException>(
            () => parser.ParseAsync(upload, stream.ToArray(), CancellationToken.None));

        Assert.Equal(LocalizationKeys.FinancialExcel.AccountNameMissing, exception.ErrorCode);
        Assert.Equal(2, exception.RowNumber);
    }

    [Fact]
    public async Task ParseAsync_FinancialPositionInvalidAssets_ThrowsSafeValidationCode(){
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Position");

        worksheet.Cell(1, 1).Value = "رقم الحساب";
        worksheet.Cell(1, 2).Value = "اسم الحساب";
        worksheet.Cell(1, 3).Value = "الأصول";
        worksheet.Cell(1, 4).Value = "الخصوم";

        worksheet.Cell(2, 1).Value = "1001";
        worksheet.Cell(2, 2).Value = "الصندوق";
        worksheet.Cell(2, 3).Value = "INVALID_SECRET_VALUE";

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var parser = new FinancialExcelParser(null!);
        var upload = new FinancialUpload {
            FileType = FinancialFileType.FinancialPosition
        };

        var exception = await Assert.ThrowsAsync<FinancialExcelValidationException>(
            () => parser.ParseAsync(upload, stream.ToArray(), CancellationToken.None));

        Assert.Equal(LocalizationKeys.FinancialExcel.InvalidNumber, exception.ErrorCode);
        Assert.Equal(2, exception.RowNumber);
        Assert.DoesNotContain("INVALID_SECRET_VALUE", exception.Message);
    }
    [Fact]
    public void ValidationException_PreservesErrorCodeAndRowNumber(){
        var exception = new FinancialExcelValidationException(
            LocalizationKeys.FinancialExcel.InvalidDate, 25);

        Assert.Equal(LocalizationKeys.FinancialExcel.InvalidDate, exception.ErrorCode);
        Assert.Equal(25, exception.RowNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidationException_RejectsInvalidRowNumber(int rowNumber){
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new FinancialExcelValidationException(
                LocalizationKeys.FinancialExcel.InvalidDate, rowNumber));
    }
}