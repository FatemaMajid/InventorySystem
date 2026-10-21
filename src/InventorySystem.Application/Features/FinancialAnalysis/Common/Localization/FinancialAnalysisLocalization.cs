using InventorySystem.Application.Common.Localization;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.Features.FinancialAnalysis.Common.Localization;

public static class FinancialAnalysisLocalization
{
    public static string NormalizeLanguage(string? language)
    {
        return string.Equals(
            language,
            "en",
            StringComparison.OrdinalIgnoreCase)
            ? "en"
            : "ar";
    }

    public static string T(
        string key,
        string? language)
    {
        var normalized =
            NormalizeLanguage(language);

        var resources =
            normalized == "en"
                ? EnglishResources.Values
                : ArabicResources.Values;

        return resources.TryGetValue(
            key,
            out var value) &&
            !string.IsNullOrWhiteSpace(value)
                ? value
                : key;
    }

    public static string Gender(
        FinancialGender gender,
        string? language)
    {
        return gender switch
        {
            FinancialGender.Male =>
                T(
                    LocalizationKeys.FinancialAnalysis.Male,
                    language),

            FinancialGender.Female =>
                T(
                    LocalizationKeys.FinancialAnalysis.Female,
                    language),

            _ =>
                gender.ToString()
        };
    }

    public static string PeriodStatus(
        FinancialPeriodStatus status,
        string? language)
    {
        var key =
            status switch
            {
                FinancialPeriodStatus.Draft =>
                    LocalizationKeys.FinancialAnalysis.Draft,

                FinancialPeriodStatus.Uploading =>
                    LocalizationKeys.FinancialAnalysis.Uploading,

                FinancialPeriodStatus.Ready =>
                    LocalizationKeys.FinancialAnalysis.Ready,

                FinancialPeriodStatus.Approved =>
                    LocalizationKeys.FinancialAnalysis.Approved,

                _ => null
            };

        return key is null
            ? status.ToString()
            : T(
                key,
                language);
    }

    public static string ProfitLossResult(
        decimal value,
        string? language)
    {
        if(value > 0)
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Profit,
                language);
        }

        if(value < 0)
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Loss,
                language);
        }

        return T(
            LocalizationKeys.FinancialAnalysis.BreakEven,
            language);
    }
}
