namespace InventorySystem.Application.Common.Localization;

public static class EnglishResources
{
    public static readonly Dictionary<string, string> Values =
        new()
        {
            [LocalizationKeys.ItemCodeRequired] =
                "Item code is required.",

            [LocalizationKeys.DuplicateItemCode] =
                "Item code is duplicated in the file.",

            [LocalizationKeys.ItemNameRequired] =
                "Item name is required.",

            [LocalizationKeys.QuantityRequired] =
                "Quantity is missing or invalid.",

            [LocalizationKeys.NegativeQuantity] =
                "Quantity cannot be negative.",

            [LocalizationKeys.PriceRequired] =
                "Price is missing or invalid.",

            [LocalizationKeys.NegativePrice] =
                "Price cannot be negative.",

            [LocalizationKeys.EmptyExcelFile] =
                "The Excel file is empty.",

            [LocalizationKeys.InvalidExcelHeaders] =
                "The required Excel columns were not found.",

            [LocalizationKeys.NoWorksheet] =
                "The Excel file does not contain any worksheet.",

            [LocalizationKeys.Increase] =
                "Increase",

            [LocalizationKeys.Decrease] =
                "Decrease",

            [LocalizationKeys.Match] =
                "Match",

            [LocalizationKeys.NewlyCounted] =
                "Newly Counted",

            [LocalizationKeys.FullyDepleted] =
                "Fully Depleted",

            [LocalizationKeys.PriceChanged] =
                "Price Changed"
        };
}
