namespace InventorySystem.Application.Common.Localization;

public static class LocalizationKeys
{
    // Common
    public static class Common
    {
        public const string NotFound = "Common.NotFound";
        public const string Invalid = "Common.Invalid";
        public const string Required = "Common.Required";
        public const string Created = "Common.Created";
        public const string Updated = "Common.Updated";
        public const string Deleted = "Common.Deleted";
        public const string ValidationFailed = "Common.ValidationFailed";
        public const string OperationFailed = "Common.OperationFailed";
        public const string Unauthorized = "Common.Unauthorized";
        public const string Forbidden = "Common.Forbidden";
        public const string Duplicate = "Common.Duplicate";
        public const string Success = "Common.Success";
    }

    // Authentication
    public static class Authentication
    {
        public const string InvalidCredentials = "Authentication.InvalidCredentials";
        public const string Unauthorized = "Authentication.Unauthorized";
        public const string LoginSuccess = "Authentication.LoginSuccess";
        public const string LogoutSuccess = "Authentication.LogoutSuccess";
        public const string UserNotFound = "Authentication.UserNotFound";
    }

    // Branch
    public static class Branch
    {
        public const string Required = "Branch.Required";
        public const string NotFound = "Branch.NotFound";
        public const string CodeExists = "Branch.CodeExists";
        public const string NameExists = "Branch.NameExists";
        public const string Created = "Branch.Created";
        public const string Updated = "Branch.Updated";
        public const string Deleted = "Branch.Deleted";
        public const string CannotDelete = "Branch.CannotDelete";

        public const string CodeRequired = "Branch.CodeRequired";
        public const string CodeMaxLength = "Branch.CodeMaxLength";
        public const string ArabicNameRequired = "Branch.ArabicNameRequired";
        public const string ArabicNameMaxLength = "Branch.ArabicNameMaxLength";
        public const string EnglishNameMaxLength = "Branch.EnglishNameMaxLength";
        public const string AddressMaxLength = "Branch.AddressMaxLength";
        public const string PhoneMaxLength = "Branch.PhoneMaxLength";
        public const string InvalidId = "Branch.InvalidId";
    }

    // Store
    public static class Store
    {
        public const string Required = "Store.Required";
        public const string NotFound = "Store.NotFound";
        public const string CodeExists = "Store.CodeExists";
        public const string NameExists = "Store.NameExists";
        public const string NotBelongToBranch = "Store.NotBelongToBranch";
        public const string Created = "Store.Created";
        public const string Updated = "Store.Updated";
        public const string Deleted = "Store.Deleted";
        public const string CannotDelete = "Store.CannotDelete";

        public const string CodeRequired = "Store.CodeRequired";
        public const string CodeMaxLength = "Store.CodeMaxLength";
        public const string ArabicNameRequired = "Store.ArabicNameRequired";
        public const string ArabicNameMaxLength = "Store.ArabicNameMaxLength";
        public const string EnglishNameMaxLength = "Store.EnglishNameMaxLength";
        public const string BranchCodeRequired = "Store.BranchCodeRequired";
        public const string BranchCodeMaxLength = "Store.BranchCodeMaxLength";
    }

    // Category
    public static class Category
    {
        public const string Required = "Category.Required";
        public const string NotFound = "Category.NotFound";
        public const string CodeExists = "Category.CodeExists";
        public const string NameExists = "Category.NameExists";
        public const string Created = "Category.Created";
        public const string Updated = "Category.Updated";
        public const string Deleted = "Category.Deleted";
    }

    // Unit
    public static class Unit
    {
        public const string Required = "Unit.Required";
        public const string NotFound = "Unit.NotFound";
        public const string Unsupported = "Unit.Unsupported";
        public const string CodeExists = "Unit.CodeExists";
        public const string NameExists = "Unit.NameExists";
        public const string Created = "Unit.Created";
        public const string Updated = "Unit.Updated";
        public const string Deleted = "Unit.Deleted";
        public const string Piece = "Unit.Piece";
        public const string Dozen = "Unit.Dozen";
    }

    // Item
    public static class Item
    {
        public const string Required = "Item.Required";
        public const string NotFound = "Item.NotFound";
        public const string CodeRequired = "Item.CodeRequired";
        public const string CodeExists = "Item.CodeExists";
        public const string NameRequired = "Item.NameRequired";
        public const string Created = "Item.Created";
        public const string Updated = "Item.Updated";
        public const string Deleted = "Item.Deleted";
    }

    // Item Location
    public static class ItemLocation
    {
        public const string NotFound = "ItemLocation.NotFound";
        public const string AlreadyExists = "ItemLocation.AlreadyExists";
        public const string Created = "ItemLocation.Created";
        public const string Deleted = "ItemLocation.Deleted";
    }

    // Price
    public static class Price
    {
        public const string Required = "Price.Required";
        public const string Invalid = "Price.Invalid";
        public const string Negative = "Price.Negative";
        public const string Updated = "Price.Updated";
        public const string HistoryNotFound = "Price.HistoryNotFound";
    }

    // Inventory
    public static class Inventory
    {
        public const string SessionNotFound = "Inventory.SessionNotFound";
        public const string SessionExists = "Inventory.SessionExists";
        public const string SessionNumberRequired = "Inventory.SessionNumberRequired";
        public const string InvalidType = "Inventory.InvalidType";
        public const string InvalidDate = "Inventory.InvalidDate";
        public const string ImportSuccess = "Inventory.ImportSuccess";
        public const string ImportFailed = "Inventory.ImportFailed";
        public const string DetailNotFound = "Inventory.DetailNotFound";
        public const string NoData = "Inventory.NoData";
        public const string Increase = "Increase";
        public const string Decrease = "Decrease";
        public const string Match = "Match";
        public const string NewlyCounted = "NewlyCounted";
        public const string FullyDepleted = "FullyDepleted";
        public const string PriceChanged = "PriceChanged";
    }

    // Excel
    public static class Excel
    {
        public const string EmptyFile = "EmptyExcelFile";
        public const string InvalidHeaders = "InvalidExcelHeaders";
        public const string NoWorksheet = "NoWorksheet";
        public const string InvalidFile = "Excel.InvalidFile";
        public const string ImportSuccess = "Excel.ImportSuccess";
        public const string ImportFailed = "Excel.ImportFailed";
        public const string UnsupportedFormat = "Excel.UnsupportedFormat";
        public const string ImportCompleted = "ImportCompleted";
        public const string InvalidExcelFile = "InvalidExcelFile";
        public const string InvalidLocationData = "InvalidLocationData";
        public const string BranchNotFound = "BranchNotFound";
        public const string StoreNotFound = "StoreNotFound";
    }

    // Reports
    public static class Report
    {
        public const string NoData = "Report.NoData";
        public const string GenerationFailed = "Report.GenerationFailed";
        public const string Generated = "Report.Generated";
    }

    // Roles
    public static class Role
    {
        public const string Required = "Role.Required";
        public const string NotFound = "Role.NotFound";
        public const string NameExists = "Role.NameExists";
        public const string Created = "Role.Created";
        public const string Updated = "Role.Updated";
        public const string Deleted = "Role.Deleted";
    }

    // Permissions
    public static class Permission
    {
        public const string NotFound = "Permission.NotFound";
        public const string Denied = "Permission.Denied";
        public const string Created = "Permission.Created";
        public const string Updated = "Permission.Updated";
        public const string Deleted = "Permission.Deleted";
    }

    // Existing keys - kept for compatibility
    public const string ItemCodeRequired = "ItemCodeRequired";
    public const string DuplicateItemCode = "DuplicateItemCode";
    public const string ItemNameRequired = "ItemNameRequired";
    public const string QuantityRequired = "QuantityRequired";
    public const string NegativeQuantity = "NegativeQuantity";
    public const string PriceRequired = "PriceRequired";
    public const string NegativePrice = "NegativePrice";
    public const string EmptyExcelFile = "EmptyExcelFile";
    public const string InvalidExcelHeaders = "InvalidExcelHeaders";
    public const string NoWorksheet = "NoWorksheet";
    public const string Increase = "Increase";
    public const string Decrease = "Decrease";
    public const string Match = "Match";
    public const string NewlyCounted = "NewlyCounted";
    public const string FullyDepleted = "FullyDepleted";
    public const string PriceChanged = "PriceChanged";
    public const string BranchRequired = "BranchRequired";
    public const string MultipleBranches = "MultipleBranches";
    public const string StoreRequired = "StoreRequired";
    public const string MultipleStores = "MultipleStores";
}