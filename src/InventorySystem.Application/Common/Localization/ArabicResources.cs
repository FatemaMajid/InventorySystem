namespace InventorySystem.Application.Common.Localization;

public static class ArabicResources
{
    public static readonly Dictionary<string, string> Values = new()
    {
        // Common
        [LocalizationKeys.Common.NotFound] = "العنصر المطلوب غير موجود.",
        [LocalizationKeys.Common.Invalid] = "البيانات غير صالحة.",
        [LocalizationKeys.Common.Required] = "هذا الحقل مطلوب.",
        [LocalizationKeys.Common.Created] = "تمت الإضافة بنجاح.",
        [LocalizationKeys.Common.Updated] = "تم التعديل بنجاح.",
        [LocalizationKeys.Common.Deleted] = "تم الحذف بنجاح.",
        [LocalizationKeys.Common.ValidationFailed] = "فشل التحقق من البيانات.",
        [LocalizationKeys.Common.OperationFailed] = "فشلت العملية.",
        [LocalizationKeys.Common.Unauthorized] = "غير مصرح لك بتنفيذ هذه العملية.",
        [LocalizationKeys.Common.Forbidden] = "ليس لديك صلاحية لتنفيذ هذه العملية.",
        [LocalizationKeys.Common.Duplicate] = "البيانات موجودة مسبقًا.",
        [LocalizationKeys.Common.Success] = "تمت العملية بنجاح.",

        // Authentication
        [LocalizationKeys.Authentication.InvalidCredentials] = "اسم المستخدم أو كلمة المرور غير صحيحة.",
        [LocalizationKeys.Authentication.Unauthorized] = "يجب تسجيل الدخول أولًا.",
        [LocalizationKeys.Authentication.LoginSuccess] = "تم تسجيل الدخول بنجاح.",
        [LocalizationKeys.Authentication.LogoutSuccess] = "تم تسجيل الخروج بنجاح.",
        [LocalizationKeys.Authentication.UserNotFound] = "المستخدم غير موجود.",

        // Branch
        [LocalizationKeys.Branch.Required] = "الفرع مطلوب.",
        [LocalizationKeys.Branch.NotFound] = "الفرع غير موجود.",
        [LocalizationKeys.Branch.CodeExists] = "رمز الفرع مستخدم مسبقًا.",
        [LocalizationKeys.Branch.NameExists] = "اسم الفرع مستخدم مسبقًا.",
        [LocalizationKeys.Branch.Created] = "تمت إضافة الفرع بنجاح.",
        [LocalizationKeys.Branch.Updated] = "تم تعديل الفرع بنجاح.",
        [LocalizationKeys.Branch.Deleted] = "تم حذف الفرع بنجاح.",
        [LocalizationKeys.Branch.CannotDelete] = "لا يمكن حذف الفرع لوجود بيانات مرتبطة به.",

        // Store
        [LocalizationKeys.Store.Required] = "المستودع مطلوب.",
        [LocalizationKeys.Store.NotFound] = "المستودع غير موجود.",
        [LocalizationKeys.Store.CodeExists] = "رمز المستودع مستخدم مسبقًا.",
        [LocalizationKeys.Store.NameExists] = "اسم المستودع مستخدم مسبقًا.",
        [LocalizationKeys.Store.NotBelongToBranch] = "المستودع لا يتبع الفرع المحدد.",
        [LocalizationKeys.Store.Created] = "تمت إضافة المستودع بنجاح.",
        [LocalizationKeys.Store.Updated] = "تم تعديل المستودع بنجاح.",
        [LocalizationKeys.Store.Deleted] = "تم حذف المستودع بنجاح.",
        [LocalizationKeys.Store.CannotDelete] = "لا يمكن حذف المستودع لوجود بيانات مرتبطة به.",

        // Category
        [LocalizationKeys.Category.Required] = "التصنيف مطلوب.",
        [LocalizationKeys.Category.NotFound] = "التصنيف غير موجود.",
        [LocalizationKeys.Category.CodeExists] = "رمز التصنيف مستخدم مسبقًا.",
        [LocalizationKeys.Category.NameExists] = "اسم التصنيف مستخدم مسبقًا.",
        [LocalizationKeys.Category.Created] = "تمت إضافة التصنيف بنجاح.",
        [LocalizationKeys.Category.Updated] = "تم تعديل التصنيف بنجاح.",
        [LocalizationKeys.Category.Deleted] = "تم حذف التصنيف بنجاح.",

        // Unit
        [LocalizationKeys.Unit.Required] = "الوحدة مطلوبة.",
        [LocalizationKeys.Unit.NotFound] = "الوحدة غير موجودة.",
        [LocalizationKeys.Unit.Unsupported] = "الوحدة غير مدعومة.",
        [LocalizationKeys.Unit.CodeExists] = "رمز الوحدة مستخدم مسبقًا.",
        [LocalizationKeys.Unit.NameExists] = "اسم الوحدة مستخدم مسبقًا.",
        [LocalizationKeys.Unit.Created] = "تمت إضافة الوحدة بنجاح.",
        [LocalizationKeys.Unit.Updated] = "تم تعديل الوحدة بنجاح.",
        [LocalizationKeys.Unit.Deleted] = "تم حذف الوحدة بنجاح.",
        [LocalizationKeys.Unit.Piece] = "قطعة",
        [LocalizationKeys.Unit.Dozen] = "درزن",

        // Item
        [LocalizationKeys.Item.Required] = "الصنف مطلوب.",
        [LocalizationKeys.Item.NotFound] = "الصنف غير موجود.",
        [LocalizationKeys.Item.CodeRequired] = "رقم الصنف مفقود.",
        [LocalizationKeys.Item.CodeExists] = "رقم الصنف مستخدم مسبقًا.",
        [LocalizationKeys.Item.NameRequired] = "اسم الصنف مفقود.",
        [LocalizationKeys.Item.Created] = "تمت إضافة الصنف بنجاح.",
        [LocalizationKeys.Item.Updated] = "تم تعديل الصنف بنجاح.",
        [LocalizationKeys.Item.Deleted] = "تم حذف الصنف بنجاح.",

        // Item Location
        [LocalizationKeys.ItemLocation.NotFound] = "موقع الصنف غير موجود.",
        [LocalizationKeys.ItemLocation.AlreadyExists] = "الصنف مرتبط بهذا الموقع مسبقًا.",
        [LocalizationKeys.ItemLocation.Created] = "تمت إضافة موقع الصنف بنجاح.",
        [LocalizationKeys.ItemLocation.Deleted] = "تم حذف موقع الصنف بنجاح.",

        // Price
        [LocalizationKeys.Price.Required] = "السعر مطلوب.",
        [LocalizationKeys.Price.Invalid] = "السعر غير صالح.",
        [LocalizationKeys.Price.Negative] = "السعر لا يمكن أن يكون سالبًا.",
        [LocalizationKeys.Price.Updated] = "تم تحديث السعر بنجاح.",
        [LocalizationKeys.Price.HistoryNotFound] = "سجل تغييرات السعر غير موجود.",

        // Inventory
        [LocalizationKeys.Inventory.SessionNotFound] = "جلسة الجرد غير موجودة.",
        [LocalizationKeys.Inventory.SessionExists] = "رقم جلسة الجرد مستخدم مسبقًا.",
        [LocalizationKeys.Inventory.SessionNumberRequired] = "رقم جلسة الجرد مطلوب.",
        [LocalizationKeys.Inventory.InvalidType] = "نوع الجرد غير صالح.",
        [LocalizationKeys.Inventory.InvalidDate] = "تاريخ الجرد غير صالح.",
        [LocalizationKeys.Inventory.ImportSuccess] = "تم استيراد الجرد بنجاح.",
        [LocalizationKeys.Inventory.ImportFailed] = "فشل استيراد الجرد.",
        [LocalizationKeys.Inventory.DetailNotFound] = "تفاصيل الجرد غير موجودة.",
        [LocalizationKeys.Inventory.NoData] = "لا توجد بيانات للجرد.",
        [LocalizationKeys.Inventory.Increase] = "زيادة",
        [LocalizationKeys.Inventory.Decrease] = "نقصان",
        [LocalizationKeys.Inventory.Match] = "مطابق",
        [LocalizationKeys.Inventory.NewlyCounted] = "ظهر بعد الجرد",
        [LocalizationKeys.Inventory.FullyDepleted] = "نفد بالكامل",
        [LocalizationKeys.Inventory.PriceChanged] = "تغير السعر",

        // Excel
        [LocalizationKeys.Excel.EmptyFile] = "ملف Excel فارغ.",
        [LocalizationKeys.Excel.InvalidHeaders] = "لم يتم العثور على أعمدة Excel المطلوبة.",
        [LocalizationKeys.Excel.NoWorksheet] = "ملف Excel لا يحتوي على أي ورقة عمل.",
        [LocalizationKeys.Excel.InvalidFile] = "ملف Excel غير صالح.",
        [LocalizationKeys.Excel.ImportSuccess] = "تم استيراد ملف Excel بنجاح.",
        [LocalizationKeys.Excel.ImportFailed] = "فشل استيراد ملف Excel.",
        [LocalizationKeys.Excel.UnsupportedFormat] = "صيغة ملف Excel غير مدعومة.",

        // Reports
        [LocalizationKeys.Report.NoData] = "لا توجد بيانات لعرض التقرير.",
        [LocalizationKeys.Report.GenerationFailed] = "فشل إنشاء التقرير.",
        [LocalizationKeys.Report.Generated] = "تم إنشاء التقرير بنجاح.",

        // Roles
        [LocalizationKeys.Role.Required] = "الدور مطلوب.",
        [LocalizationKeys.Role.NotFound] = "الدور غير موجود.",
        [LocalizationKeys.Role.NameExists] = "اسم الدور مستخدم مسبقًا.",
        [LocalizationKeys.Role.Created] = "تمت إضافة الدور بنجاح.",
        [LocalizationKeys.Role.Updated] = "تم تعديل الدور بنجاح.",
        [LocalizationKeys.Role.Deleted] = "تم حذف الدور بنجاح.",

        // Permissions
        [LocalizationKeys.Permission.NotFound] = "الصلاحية غير موجودة.",
        [LocalizationKeys.Permission.Denied] = "ليس لديك الصلاحية لتنفيذ هذه العملية.",
        [LocalizationKeys.Permission.Created] = "تمت إضافة الصلاحية بنجاح.",
        [LocalizationKeys.Permission.Updated] = "تم تعديل الصلاحية بنجاح.",
        [LocalizationKeys.Permission.Deleted] = "تم حذف الصلاحية بنجاح.",

        // Existing keys - compatibility
        [LocalizationKeys.ItemCodeRequired] = "رقم الصنف مفقود.",
        [LocalizationKeys.DuplicateItemCode] = "رقم الصنف مكرر داخل الملف.",
        [LocalizationKeys.ItemNameRequired] = "اسم الصنف مفقود.",
        [LocalizationKeys.QuantityRequired] = "الكمية مفقودة أو غير صالحة.",
        [LocalizationKeys.NegativeQuantity] = "الكمية لا يمكن أن تكون سالبة.",
        [LocalizationKeys.PriceRequired] = "السعر مفقود أو غير صالح.",
        [LocalizationKeys.NegativePrice] = "السعر لا يمكن أن يكون سالبًا.",
        [LocalizationKeys.EmptyExcelFile] = "ملف Excel فارغ.",
        [LocalizationKeys.InvalidExcelHeaders] = "لم يتم العثور على أعمدة Excel المطلوبة.",
        [LocalizationKeys.NoWorksheet] = "ملف Excel لا يحتوي على أي ورقة عمل.",
        [LocalizationKeys.Increase] = "زيادة",
        [LocalizationKeys.Decrease] = "نقصان",
        [LocalizationKeys.Match] = "مطابق",
        [LocalizationKeys.NewlyCounted] = "ظهر بعد الجرد",
        [LocalizationKeys.FullyDepleted] = "نفد بالكامل",
        [LocalizationKeys.PriceChanged] = "تغير السعر",
        [LocalizationKeys.BranchRequired] = "الفرع مفقود.",
        [LocalizationKeys.MultipleBranches] = "الملف يحتوي على أكثر من فرع.",
        [LocalizationKeys.StoreRequired] = "المستودع مفقود.",
        [LocalizationKeys.MultipleStores] = "الملف يحتوي على أكثر من مستودع."
    };
}