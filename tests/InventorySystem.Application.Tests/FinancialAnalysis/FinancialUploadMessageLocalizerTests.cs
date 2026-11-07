using System.Globalization;
using InventorySystem.Application.Features.FinancialAnalysis.Common.Localization;
using Xunit;

namespace InventorySystem.Application.Tests.FinancialAnalysis;

public class FinancialUploadMessageLocalizerTests {
    [Theory]
    [InlineData("ar", "FinancialExcel.InvalidDate:25", "تاريخ غير صالح في الصف 25.")]
    [InlineData("en", "FinancialExcel.InvalidDate:25", "Invalid date in row 25.")]
    [InlineData("ar", "FinancialExcel.NoTransferRecords", "ملف التحويلات لا يحتوي على سجلات فروع.")]
    [InlineData("en", "FinancialExcel.NoTransferRecords", "The transfer file contains no branch records.")]
    [InlineData("en", "FinancialUpload.ParsedRows:39", "File validated successfully. Parsed rows: 39.")]
    [InlineData("en", "Valid. Parsed rows: 39.", "File validated successfully. Parsed rows: 39.")]
    public void Resolve_ReturnsLocalizedMessage(string language, string storedMessage, string expected){
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUICulture = CultureInfo.CurrentUICulture;

        try {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(language);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(language);

            var result = FinancialUploadMessageLocalizer.Resolve(storedMessage);

            Assert.Equal(expected, result);
        }
        finally {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }

    [Theory]
    [InlineData("FinancialExcel.UnknownError")]
    [InlineData("FinancialExcel.InvalidDate:0")]
    [InlineData("FinancialExcel.InvalidDate:abc")]
    [InlineData("Invalid numeric value: SECRET_ACCOUNT_123")]
    public void Resolve_ReturnsSafeFallbackForUnknownMessages(string storedMessage){
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUICulture = CultureInfo.CurrentUICulture;

        try {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");

            var result = FinancialUploadMessageLocalizer.Resolve(storedMessage);

            Assert.Equal("Excel file import failed.", result);
            Assert.DoesNotContain("SECRET_ACCOUNT_123", result!);
        }
        finally {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }
}