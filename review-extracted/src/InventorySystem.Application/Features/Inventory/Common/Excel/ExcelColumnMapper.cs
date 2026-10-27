namespace InventorySystem.Application.Common.Excel;

public static class ExcelColumnMapper
{
    private static readonly Dictionary<string, string> ColumnAliases =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Item Code
            ["رقم الصنف"] = "ItemCode",
            ["كود الصنف"] = "ItemCode",
            ["كود"] = "ItemCode",
            ["item code"] = "ItemCode",
            ["itemcode"] = "ItemCode",

            // Item Name 1
            ["اسم الصنف"] = "ItemName1",
            ["اسم الصنف الأول"] = "ItemName1",
            ["الاسم"] = "ItemName1",
            ["item name"] = "ItemName1",
            ["itemname"] = "ItemName1",

            // Item Name 2 - Optional
            ["اسم الصنف الثاني"] = "ItemName2",
            ["اسم الصنف2"] = "ItemName2",
            ["اسم ثاني الصنف"] = "ItemName2",
            ["الاسم الثاني"] = "ItemName2",
            ["item name 2"] = "ItemName2",
            ["itemname2"] = "ItemName2",

            // Category
            ["الصنف"] = "Category",
            ["التصنيف"] = "Category",
            ["المجموعة"] = "Category",
            ["category"] = "Category",

            // Unit
            ["الوحدة"] = "Unit",
            ["وحدة القياس"] = "Unit",
            ["unit"] = "Unit",

            // Branch
            ["الفرع"] = "Branch",
            ["branch"] = "Branch",

            // Store
            ["المستودع"] = "Store",
            ["المخزن"] = "Store",
            ["المخزن"] = "Store",
            ["store"] = "Store",

            // Quantity
            ["الكمية"] = "Quantity",
            ["الكمية الفعلية"] = "Quantity",
            ["إجمالي الكميات"] = "Quantity",
            ["العدد"] = "Quantity",
            ["quantity"] = "Quantity",

            // Price
            ["السعر"] = "Price",
            ["price"] = "Price"
        };

    public static string? Map(string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
            return null;

        var normalizedName = Normalize(columnName);

        foreach (var alias in ColumnAliases)
        {
            if (Normalize(alias.Key) == normalizedName)
                return alias.Value;
        }

        return null;
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace(" ", "")
            .Replace("ـ", "")
            .ToLowerInvariant();
    }
}