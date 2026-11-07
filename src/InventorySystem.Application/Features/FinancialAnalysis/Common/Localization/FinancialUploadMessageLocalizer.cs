using System.Globalization;

using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Features.FinancialAnalysis.Common.Localization;

public static class FinancialUploadMessageLocalizer {
    public static string? Resolve(string? message){
        if (string.IsNullOrEmpty(message)){
            return message;
        }

        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        if (message is LocalizationKeys.Excel.ImportFailed
            or LocalizationKeys.Excel.EmptyFile
            or LocalizationKeys.Excel.NoWorksheet
            or LocalizationKeys.Excel.InvalidHeaders
            or LocalizationKeys.Excel.UnsupportedFormat){
            return LocalizationService.Get(message, language);
        }

        var prefix = LocalizationKeys.FinancialUpload.ParsedRows + ":";

        if (message.StartsWith(prefix, StringComparison.Ordinal) &&
            int.TryParse(message[prefix.Length..], NumberStyles.None,
                CultureInfo.InvariantCulture, out var count)){
            return string.Format(CultureInfo.CurrentUICulture,
                LocalizationService.Get(LocalizationKeys.FinancialUpload.ParsedRows, language), count);
        }

        if (message.StartsWith("FinancialExcel.", StringComparison.Ordinal)){
            var separator = message.LastIndexOf(':');
            var code = separator >= 0 ? message[..separator] : message;
            var template = LocalizationService.Get(code, language);

            if (template != code){
                var requiresRow = template.Contains("{0}", StringComparison.Ordinal);
                if (!requiresRow && separator < 0){
                    return template;
                }
                if (requiresRow && separator > 0 &&
                    int.TryParse(message[(separator + 1)..], NumberStyles.None,
                        CultureInfo.InvariantCulture, out var rowNumber) && rowNumber > 0){
                    return string.Format(CultureInfo.CurrentUICulture, template, rowNumber);
                }
            }
        }
        var legacyPrefix = "Valid. Parsed rows: ";
        if (message.StartsWith(legacyPrefix, StringComparison.Ordinal) &&
            message.EndsWith(".", StringComparison.Ordinal) &&
            int.TryParse(message[legacyPrefix.Length..^1], NumberStyles.None,
                CultureInfo.InvariantCulture, out var legacyCount)){
            return string.Format(CultureInfo.CurrentUICulture,
                LocalizationService.Get(LocalizationKeys.FinancialUpload.ParsedRows, language), legacyCount);
        }

        return LocalizationService.Get(LocalizationKeys.Excel.ImportFailed, language);
    }
}