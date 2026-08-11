namespace InventorySystem.Application.Common.Localization;

public static class ArabicResources
{
    public static readonly Dictionary<string, string> Values =
        new()
        {
            [LocalizationKeys.ItemCodeRequired] =
                "رقم الصنف مفقود.",

            [LocalizationKeys.DuplicateItemCode] =
                "رقم الصنف مكرر داخل الملف.",

            [LocalizationKeys.ItemNameRequired] =
                "اسم الصنف مفقود.",

            [LocalizationKeys.QuantityRequired] =
                "الكمية مفقودة أو غير صالحة.",

            [LocalizationKeys.NegativeQuantity] =
                "الكمية لا يمكن أن تكون سالبة.",

            [LocalizationKeys.PriceRequired] =
                "السعر مفقود أو غير صالح.",

            [LocalizationKeys.NegativePrice] =
                "السعر لا يمكن أن يكون سالبًا.",

            [LocalizationKeys.EmptyExcelFile] =
                "ملف Excel فارغ.",

            [LocalizationKeys.InvalidExcelHeaders] =
                "لم يتم العثور على أعمدة Excel المطلوبة.",

            [LocalizationKeys.NoWorksheet] =
                "ملف Excel لا يحتوي على أي ورقة عمل.",

            [LocalizationKeys.Increase] =
                "زيادة",

            [LocalizationKeys.Decrease] =
                "نقصان",

            [LocalizationKeys.Match] =
                "مطابق",

            [LocalizationKeys.NewlyCounted] =
                "ظهر بعد الجرد",

            [LocalizationKeys.FullyDepleted] =
                "نفد بالكامل",

            [LocalizationKeys.PriceChanged] =
                "تغير السعر"
        };
}