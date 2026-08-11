namespace InventorySystem.Application.Common.Localization;

public static class LocalizationService
{
    public static string Get(
        string key,
        string language = "ar")
    {
        var resources = language.ToLowerInvariant() switch
        {
            "en" => EnglishResources.Values,
            "ar" => ArabicResources.Values,
            _ => ArabicResources.Values
        };

        return resources.TryGetValue(key, out var value)
            ? value
            : key;
    }
}