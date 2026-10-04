using System.Globalization;

namespace InventorySystem.Application.Common.Excel.Normalization;

public static class UnitNormalizer
{
    public static string? Normalize(string? unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
            return null;

        var normalized = new string(
            unit
                .Trim()
                .Where(c =>
                    !char.IsWhiteSpace(c) &&
                    char.GetUnicodeCategory(c) != UnicodeCategory.Format &&
                    char.GetUnicodeCategory(c) != UnicodeCategory.Control &&
                    c != 'ـ')
                .ToArray());

        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        return normalized switch
        {
            "ق" => "قطعة",
            "قطعة" => "قطعة",
            "قطعه" => "قطعة",
            "قطعةد" => "قطعة",

            "د" => "درزن",
            "درزن" => "درزن",

            "سيت" => "سيت",
            "سيت6ق" => "سيت",

            "غم" => "غم",
            "سم" => "سم",
            "كغم" => "كغم",
            "علبة" => "علبة",

            _ => normalized
        };
    }
}