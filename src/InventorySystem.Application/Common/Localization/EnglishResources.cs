namespace InventorySystem.Application.Common.Localization;

public static class EnglishResources
{
    public static readonly Dictionary<string, string> Values = new()
    {
        // Common
        [LocalizationKeys.Common.NotFound] = "The requested item was not found.",
        [LocalizationKeys.Common.Invalid] = "The data is invalid.",
        [LocalizationKeys.Common.Required] = "This field is required.",
        [LocalizationKeys.Common.Created] = "Created successfully.",
        [LocalizationKeys.Common.Updated] = "Updated successfully.",
        [LocalizationKeys.Common.Deleted] = "Deleted successfully.",
        [LocalizationKeys.Common.ValidationFailed] = "Validation failed.",
        [LocalizationKeys.Common.OperationFailed] = "The operation failed.",
        [LocalizationKeys.Common.Unauthorized] = "You are not authorized to perform this operation.",
        [LocalizationKeys.Common.Forbidden] = "You do not have permission to perform this operation.",
        [LocalizationKeys.Common.Duplicate] = "The data already exists.",
        [LocalizationKeys.Common.Success] = "Operation completed successfully.",

        // Authentication
        [LocalizationKeys.Authentication.InvalidCredentials] = "Invalid username or password.",
        [LocalizationKeys.Authentication.Unauthorized] = "You must log in first.",
        [LocalizationKeys.Authentication.LoginSuccess] = "Logged in successfully.",
        [LocalizationKeys.Authentication.LogoutSuccess] = "Logged out successfully.",
        [LocalizationKeys.Authentication.UserNotFound] = "User not found.",

        // Branch
        [LocalizationKeys.Branch.Required] = "Branch is required.",
        [LocalizationKeys.Branch.NotFound] = "Branch not found.",
        [LocalizationKeys.Branch.CodeExists] = "Branch code already exists.",
        [LocalizationKeys.Branch.NameExists] = "Branch name already exists.",
        [LocalizationKeys.Branch.Created] = "Branch created successfully.",
        [LocalizationKeys.Branch.Updated] = "Branch updated successfully.",
        [LocalizationKeys.Branch.Deleted] = "Branch deleted successfully.",
        [LocalizationKeys.Branch.CannotDelete] = "The branch cannot be deleted because it has related data.",

        // Branch validation
        [LocalizationKeys.Branch.CodeRequired] = "Branch code is required.",
        [LocalizationKeys.Branch.CodeMaxLength] = "Branch code must not exceed 20 characters.",
        [LocalizationKeys.Branch.ArabicNameRequired] = "Branch Arabic name is required.",
        [LocalizationKeys.Branch.ArabicNameMaxLength] = "Branch Arabic name must not exceed 100 characters.",
        [LocalizationKeys.Branch.EnglishNameMaxLength] = "Branch English name must not exceed 100 characters.",
        [LocalizationKeys.Branch.AddressMaxLength] = "Address must not exceed 200 characters.",
        [LocalizationKeys.Branch.PhoneMaxLength] = "Phone number must not exceed 20 characters.",
        [LocalizationKeys.Branch.InvalidId] = "Branch ID must be greater than zero.",
        
        // Store
        [LocalizationKeys.Store.Required] = "Store is required.",
        [LocalizationKeys.Store.NotFound] = "Store not found.",
        [LocalizationKeys.Store.CodeExists] = "Store code already exists.",
        [LocalizationKeys.Store.NameExists] = "Store name already exists.",
        [LocalizationKeys.Store.NotBelongToBranch] = "The store does not belong to the selected branch.",
        [LocalizationKeys.Store.Created] = "Store created successfully.",
        [LocalizationKeys.Store.Updated] = "Store updated successfully.",
        [LocalizationKeys.Store.Deleted] = "Store deleted successfully.",
        [LocalizationKeys.Store.CannotDelete] = "The store cannot be deleted because it has related data.",

        // Store validation
        [LocalizationKeys.Store.CodeRequired] = "Store code is required.",
        [LocalizationKeys.Store.CodeMaxLength] = "Store code must not exceed 20 characters.",
        [LocalizationKeys.Store.ArabicNameRequired] = "Store Arabic name is required.",
        [LocalizationKeys.Store.ArabicNameMaxLength] = "Store Arabic name must not exceed 100 characters.",
        [LocalizationKeys.Store.EnglishNameMaxLength] = "Store English name must not exceed 100 characters.",
        [LocalizationKeys.Store.BranchCodeRequired] = "Branch code is required.",
        [LocalizationKeys.Store.BranchCodeMaxLength] = "Branch code must not exceed 20 characters.",

        // Category
        [LocalizationKeys.Category.Required] = "Category is required.",
        [LocalizationKeys.Category.NotFound] = "Category not found.",
        [LocalizationKeys.Category.CodeExists] = "Category code already exists.",
        [LocalizationKeys.Category.NameExists] = "Category name already exists.",
        [LocalizationKeys.Category.Created] = "Category created successfully.",
        [LocalizationKeys.Category.Updated] = "Category updated successfully.",
        [LocalizationKeys.Category.Deleted] = "Category deleted successfully.",

        // Unit
        [LocalizationKeys.Unit.Required] = "Unit is required.",
        [LocalizationKeys.Unit.NotFound] = "Unit not found.",
        [LocalizationKeys.Unit.Unsupported] = "The specified unit is not supported.",
        [LocalizationKeys.Unit.CodeExists] = "Unit code already exists.",
        [LocalizationKeys.Unit.NameExists] = "Unit name already exists.",
        [LocalizationKeys.Unit.Created] = "Unit created successfully.",
        [LocalizationKeys.Unit.Updated] = "Unit updated successfully.",
        [LocalizationKeys.Unit.Deleted] = "Unit deleted successfully.",
        [LocalizationKeys.Unit.Piece] = "Piece",
        [LocalizationKeys.Unit.Dozen] = "Dozen",
        [LocalizationKeys.Unit.FirstUnitNotDefined] = "The first unit is not defined.",

        // Item
        [LocalizationKeys.Item.Required] = "Item is required.",
        [LocalizationKeys.Item.NotFound] = "Item not found.",
        [LocalizationKeys.Item.CodeRequired] = "Item code is required.",
        [LocalizationKeys.Item.CodeExists] = "Item code already exists.",
        [LocalizationKeys.Item.NameRequired] = "Item name is required.",
        [LocalizationKeys.Item.Created] = "Item created successfully.",
        [LocalizationKeys.Item.Updated] = "Item updated successfully.",
        [LocalizationKeys.Item.Deleted] = "Item deleted successfully.",

        // Item Location
        [LocalizationKeys.ItemLocation.NotFound] = "Item location not found.",
        [LocalizationKeys.ItemLocation.AlreadyExists] = "The item is already assigned to this location.",
        [LocalizationKeys.ItemLocation.Created] = "Item location created successfully.",
        [LocalizationKeys.ItemLocation.Deleted] = "Item location deleted successfully.",

        // Price
        [LocalizationKeys.Price.Required] = "Price is required.",
        [LocalizationKeys.Price.Invalid] = "Price is invalid.",
        [LocalizationKeys.Price.Negative] = "Price cannot be negative.",
        [LocalizationKeys.Price.Updated] = "Price updated successfully.",
        [LocalizationKeys.Price.HistoryNotFound] = "Price history was not found.",

        // Inventory
        [LocalizationKeys.Inventory.SessionNotFound] = "Inventory session not found.",
        [LocalizationKeys.Inventory.SessionExists] = "Inventory session number already exists.",
        [LocalizationKeys.Inventory.SessionNumberRequired] = "Inventory session number is required.",
        [LocalizationKeys.Inventory.InvalidType] = "Invalid inventory type.",
        [LocalizationKeys.Inventory.InvalidDate] = "Invalid inventory date.",
        [LocalizationKeys.Inventory.ImportSuccess] = "Inventory imported successfully.",
        [LocalizationKeys.Inventory.ImportFailed] = "Inventory import failed.",
        [LocalizationKeys.Inventory.DetailNotFound] = "Inventory details not found.",
        [LocalizationKeys.Inventory.NoData] = "No inventory data found.",
        [LocalizationKeys.Inventory.Increase] = "Increase",
        [LocalizationKeys.Inventory.Decrease] = "Decrease",
        [LocalizationKeys.Inventory.Match] = "Match",
        [LocalizationKeys.Inventory.NewlyCounted] = "Newly Counted",
        [LocalizationKeys.Inventory.FullyDepleted] = "Fully Depleted",
        [LocalizationKeys.Inventory.PriceChanged] = "Price Changed",

        // Excel
        [LocalizationKeys.Excel.EmptyFile] = "The Excel file is empty.",
        [LocalizationKeys.Excel.InvalidHeaders] = "The required Excel columns were not found.",
        [LocalizationKeys.Excel.NoWorksheet] = "The Excel file does not contain any worksheet.",
        [LocalizationKeys.Excel.InvalidFile] = "The Excel file is invalid.",
        [LocalizationKeys.Excel.ImportSuccess] = "Excel file imported successfully.",
        [LocalizationKeys.Excel.ImportFailed] = "Excel file import failed.",
        [LocalizationKeys.Excel.UnsupportedFormat] = "The Excel file format is not supported.",

        // Reports
        [LocalizationKeys.Report.NoData] = "No data is available for the report.",
        [LocalizationKeys.Report.GenerationFailed] = "Failed to generate the report.",
        [LocalizationKeys.Report.Generated] = "Report generated successfully.",

        // Roles
        [LocalizationKeys.Role.Required] = "Role is required.",
        [LocalizationKeys.Role.NotFound] = "Role not found.",
        [LocalizationKeys.Role.NameExists] = "Role name already exists.",
        [LocalizationKeys.Role.Created] = "Role created successfully.",
        [LocalizationKeys.Role.Updated] = "Role updated successfully.",
        [LocalizationKeys.Role.Deleted] = "Role deleted successfully.",

        // Permissions
        [LocalizationKeys.Permission.NotFound] = "Permission not found.",
        [LocalizationKeys.Permission.Denied] = "You do not have permission to perform this operation.",
        [LocalizationKeys.Permission.Created] = "Permission created successfully.",
        [LocalizationKeys.Permission.Updated] = "Permission updated successfully.",
        [LocalizationKeys.Permission.Deleted] = "Permission deleted successfully.",

        // Existing keys - compatibility
        [LocalizationKeys.ItemCodeRequired] = "Item code is required.",
        [LocalizationKeys.DuplicateItemCode] = "Item code is duplicated in the file.",
        [LocalizationKeys.ItemNameRequired] = "Item name is required.",
        [LocalizationKeys.QuantityRequired] = "Quantity is missing or invalid.",
        [LocalizationKeys.NegativeQuantity] = "Quantity cannot be negative.",
        [LocalizationKeys.PriceRequired] = "Price is missing or invalid.",
        [LocalizationKeys.NegativePrice] = "Price cannot be negative.",
        [LocalizationKeys.EmptyExcelFile] = "The Excel file is empty.",
        [LocalizationKeys.InvalidExcelHeaders] = "The required Excel columns were not found.",
        [LocalizationKeys.NoWorksheet] = "The Excel file does not contain any worksheet.",
        [LocalizationKeys.Increase] = "Increase",
        [LocalizationKeys.Decrease] = "Decrease",
        [LocalizationKeys.Match] = "Match",
        [LocalizationKeys.NewlyCounted] = "Newly Counted",
        [LocalizationKeys.FullyDepleted] = "Fully Depleted",
        [LocalizationKeys.PriceChanged] = "Price Changed",
        [LocalizationKeys.BranchRequired] = "Branch is required.",
        [LocalizationKeys.MultipleBranches] = "The file contains more than one branch.",
        [LocalizationKeys.StoreRequired] = "Store is required.",
        [LocalizationKeys.MultipleStores] = "The file contains more than one store."
    };
}