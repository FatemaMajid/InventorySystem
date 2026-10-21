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
        public const string NotMatch = "Branch.NotMatch";
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
        public const string NotMatch = "Store.NotMatch";
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
        public const string FirstUnitNotDefined = "Unit.FirstUnitNotDefined";
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
        public const string ItemExistsBeforeOnly = "Inventory.ItemExistsBeforeOnly";
        public const string ItemExistsAfterOnly = "Inventory.ItemExistsAfterOnly";
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

    // Dashboard export (independent from generic Inventory labels)
    public static class DashboardExport
    {
        public const string Title = "DashboardExport.Title";
        public const string SessionInfo = "DashboardExport.SessionInfo";
        public const string SessionNumber = "DashboardExport.SessionNumber";
        public const string Status = "DashboardExport.Status";
        public const string InventoryType = "DashboardExport.InventoryType";
        public const string InventoryDate = "DashboardExport.InventoryDate";
        public const string Branch = "DashboardExport.Branch";
        public const string Store = "DashboardExport.Store";
        public const string BeforeFile = "DashboardExport.BeforeFile";
        public const string AfterFile = "DashboardExport.AfterFile";
        public const string Summary = "DashboardExport.Summary";
        public const string TotalItems = "DashboardExport.TotalItems";
        public const string Increase = "DashboardExport.Increase";
        public const string Decrease = "DashboardExport.Decrease";
        public const string Match = "DashboardExport.Match";
        public const string NewlyCounted = "DashboardExport.NewlyCounted";
        public const string FullyDepleted = "DashboardExport.FullyDepleted";
        public const string PriceChanged = "DashboardExport.PriceChanged";
        public const string UnitNotDefined = "DashboardExport.UnitNotDefined";
        public const string Quantity = "DashboardExport.Quantity";
        public const string QuantityBefore = "DashboardExport.QuantityBefore";
        public const string QuantityAfter = "DashboardExport.QuantityAfter";
        public const string QuantityDifference = "DashboardExport.QuantityDifference";
        public const string QuantityIncrease = "DashboardExport.QuantityIncrease";
        public const string QuantityDecrease = "DashboardExport.QuantityDecrease";
        public const string Financial = "DashboardExport.Financial";
        public const string ValueBefore = "DashboardExport.ValueBefore";
        public const string ValueAfter = "DashboardExport.ValueAfter";
        public const string Difference = "DashboardExport.Difference";
        public const string DifferencePercent = "DashboardExport.DifferencePercent";
        public const string Statuses = "DashboardExport.Statuses";
        public const string Count = "DashboardExport.Count";
        public const string Percentage = "DashboardExport.Percentage";
        public const string Attention = "DashboardExport.Attention";
        public const string Total = "DashboardExport.Total";
        public const string TopDifferences = "DashboardExport.TopDifferences";
        public const string ItemCode = "DashboardExport.ItemCode";
        public const string ItemName = "DashboardExport.ItemName";
        public const string ValueDifference = "DashboardExport.ValueDifference";
        public const string Generated = "DashboardExport.Generated";
        public const string Page = "DashboardExport.Page";
        public const string Of = "DashboardExport.Of";
        public const string Completed = "DashboardExport.Completed";
        public const string Pending = "DashboardExport.Pending";
        public const string InProgress = "DashboardExport.InProgress";
        public const string Draft = "DashboardExport.Draft";
        public const string Cancelled = "DashboardExport.Cancelled";
        public const string Canceled = "DashboardExport.Canceled";
        public const string Annual = "DashboardExport.Annual";
        public const string SemiAnnual = "DashboardExport.SemiAnnual";
        public const string StatusIncrease = "DashboardExport.StatusIncrease";
        public const string StatusDecrease = "DashboardExport.StatusDecrease";
        public const string StatusMatch = "DashboardExport.StatusMatch";
        public const string StatusNewlyCounted = "DashboardExport.StatusNewlyCounted";
        public const string StatusFullyDepleted = "DashboardExport.StatusFullyDepleted";
        public const string NoData = "DashboardExport.NoData";
        public const string AttentionNote = "DashboardExport.AttentionNote";
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

    // Financial Period
    public static class FinancialPeriod
    {
        public const string InvalidYear = "FinancialPeriod.InvalidYear";
        public const string InvalidMonth = "FinancialPeriod.InvalidMonth";
        public const string InvalidId = "FinancialPeriod.InvalidId";
        public const string AlreadyExists = "FinancialPeriod.AlreadyExists";
        public const string AlreadyApproved = "FinancialPeriod.AlreadyApproved";
        public const string FilesIncomplete = "FinancialPeriod.FilesIncomplete";
        public const string Created = "FinancialPeriod.Created";
        public const string Approved = "FinancialPeriod.Approved";
        public const string NotFound = "FinancialPeriod.NotFound";
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

    // Financial Export
    public static class FinancialExport
    {
        public const string InvalidMode = "FinancialExport.InvalidMode";
        public const string BranchRequired = "FinancialExport.BranchRequired";
        public const string PeriodNotFound = "FinancialExport.PeriodNotFound";
        public const string BranchNotFound = "FinancialExport.BranchNotFound";

        public const string Summary = "FinancialExport.Summary";
        public const string BranchComparison = "FinancialExport.BranchComparison";
        public const string IncomeStatements = "FinancialExport.IncomeStatements";
        public const string IncomeStatement = "FinancialExport.IncomeStatement";
        public const string FinancialPositions = "FinancialExport.FinancialPositions";
        public const string FinancialPosition = "FinancialExport.FinancialPosition";
        public const string TransfersMale = "FinancialExport.TransfersMale";
        public const string TransfersFemale = "FinancialExport.TransfersFemale";
        public const string SettlementsMale = "FinancialExport.SettlementsMale";
        public const string SettlementsFemale = "FinancialExport.SettlementsFemale";
        public const string BranchStatus = "FinancialExport.BranchStatus";
        public const string MissingFilesSheet = "FinancialExport.MissingFilesSheet";

        public const string AllBranchesReport = "FinancialExport.AllBranchesReport";
        public const string BranchReport = "FinancialExport.BranchReport";
        public const string BranchAnalyticalComparison = "FinancialExport.BranchAnalyticalComparison";
        public const string AllBranchFilesStatus = "FinancialExport.AllBranchFilesStatus";
        public const string RequiredMissingFiles = "FinancialExport.RequiredMissingFiles";

        public const string PeriodId = "FinancialExport.PeriodId";
        public const string StartDate = "FinancialExport.StartDate";
        public const string EndDate = "FinancialExport.EndDate";
        public const string Status = "FinancialExport.Status";
        public const string GeneratedAtBaghdad = "FinancialExport.GeneratedAtBaghdad";

        public const string FilesAndBranchesStatus = "FinancialExport.FilesAndBranchesStatus";
        public const string FilesStatus = "FinancialExport.FilesStatus";
        public const string ActiveBranches = "FinancialExport.ActiveBranches";
        public const string CompleteBranches = "FinancialExport.CompleteBranches";
        public const string IncompleteBranches = "FinancialExport.IncompleteBranches";
        public const string ExpectedFiles = "FinancialExport.ExpectedFiles";
        public const string ValidFiles = "FinancialExport.ValidFiles";
        public const string CompletedFiles = "FinancialExport.CompletedFiles";
        public const string MissingFiles = "FinancialExport.MissingFiles";
        public const string Completion = "FinancialExport.Completion";

        public const string TransferAnalysis = "FinancialExport.TransferAnalysis";
        public const string TransferMale = "FinancialExport.TransferMale";
        public const string TransferFemale = "FinancialExport.TransferFemale";
        public const string TotalTransfers = "FinancialExport.TotalTransfers";
        public const string AverageTransfersPerBranch = "FinancialExport.AverageTransfersPerBranch";
        public const string HighestTransferBranch = "FinancialExport.HighestTransferBranch";
        public const string LowestTransferBranch = "FinancialExport.LowestTransferBranch";
        public const string AverageTransferRecord = "FinancialExport.AverageTransferRecord";
        public const string HighestTransferRecord = "FinancialExport.HighestTransferRecord";
        public const string LowestTransferRecord = "FinancialExport.LowestTransferRecord";

        public const string SettlementAnalysis = "FinancialExport.SettlementAnalysis";
        public const string TotalDebit = "FinancialExport.TotalDebit";
        public const string TotalCredit = "FinancialExport.TotalCredit";
        public const string TotalBalance = "FinancialExport.TotalBalance";
        public const string Balance = "FinancialExport.Balance";
        public const string AverageBalancePerBranch = "FinancialExport.AverageBalancePerBranch";
        public const string HighestBranchBalance = "FinancialExport.HighestBranchBalance";
        public const string LowestBranchBalance = "FinancialExport.LowestBranchBalance";

        public const string BranchCode = "FinancialExport.BranchCode";
        public const string Branch = "FinancialExport.Branch";
        public const string AccountCode = "FinancialExport.AccountCode";
        public const string AccountName = "FinancialExport.AccountName";
        public const string Debit = "FinancialExport.Debit";
        public const string Credit = "FinancialExport.Credit";
        public const string Assets = "FinancialExport.Assets";
        public const string Liabilities = "FinancialExport.Liabilities";
        public const string RowOrder = "FinancialExport.RowOrder";
        public const string Gender = "FinancialExport.Gender";
        public const string Value = "FinancialExport.Value";
        public const string TransactionDate = "FinancialExport.TransactionDate";
        public const string Description = "FinancialExport.Description";
        public const string EntryNumber = "FinancialExport.EntryNumber";
        public const string Marked = "FinancialExport.Marked";
        public const string Matched = "FinancialExport.Matched";
        public const string FileType = "FinancialExport.FileType";

        public const string IncomeStatementFile = "FinancialExport.IncomeStatementFile";
        public const string FinancialPositionFile = "FinancialExport.FinancialPositionFile";
        public const string TransferFile = "FinancialExport.TransferFile";
        public const string SettlementFile = "FinancialExport.SettlementFile";

        public const string Male = "FinancialExport.Male";
        public const string Female = "FinancialExport.Female";
        public const string Yes = "FinancialExport.Yes";
        public const string No = "FinancialExport.No";
        public const string Complete = "FinancialExport.Complete";
        public const string Incomplete = "FinancialExport.Incomplete";
        public const string AllBranches = "FinancialExport.AllBranches";

        public const string Draft = "FinancialExport.Draft";
        public const string Uploading = "FinancialExport.Uploading";
        public const string Ready = "FinancialExport.Ready";
        public const string Approved = "FinancialExport.Approved";

        public const string Total = "FinancialExport.Total";
        public const string Highest = "FinancialExport.Highest";
        public const string Lowest = "FinancialExport.Lowest";
        public const string Average = "FinancialExport.Average";
        public const string Records = "FinancialExport.Records";

        public const string NoteNoAccountGrandTotal = "FinancialExport.NoteNoAccountGrandTotal";
    }

    public static class FinancialAnalysis
    {
        public const string ResultType = "FinancialAnalysis.ResultType";
        public const string ResultValue = "FinancialAnalysis.ResultValue";
        public const string SignedValue = "FinancialAnalysis.SignedValue";
        public const string Assets = "FinancialAnalysis.Assets";
        public const string Liabilities = "FinancialAnalysis.Liabilities";
        public const string AccountCode = "FinancialAnalysis.AccountCode";
        public const string AccountName = "FinancialAnalysis.AccountName";
        public const string FileAvailable = "FinancialAnalysis.FileAvailable";

        public const string Records = "FinancialAnalysis.Records";
        public const string Yes = "FinancialAnalysis.Yes";
        public const string No = "FinancialAnalysis.No";

        public const string Status = "FinancialAnalysis.Status";
        public const string NoData = "FinancialAnalysis.NoData";
        public const string CompleteBranches = "FinancialAnalysis.CompleteBranches";
        public const string IncompleteBranches = "FinancialAnalysis.IncompleteBranches";

        public const string Transactions = "FinancialAnalysis.Transactions";
        public const string FemaleFiles = "FinancialAnalysis.FemaleFiles";
        public const string MaleFiles = "FinancialAnalysis.MaleFiles";
        public const string OverallResult = "FinancialAnalysis.OverallResult";
        public const string TotalLoss = "FinancialAnalysis.TotalLoss";
        public const string TotalProfit = "FinancialAnalysis.TotalProfit";
        public const string BreakEvenBranches = "FinancialAnalysis.BreakEvenBranches";
        public const string LossBranches = "FinancialAnalysis.LossBranches";
        public const string ProfitableBranches = "FinancialAnalysis.ProfitableBranches";
        public const string BranchesWithoutData = "FinancialAnalysis.BranchesWithoutData";
        public const string BranchesWithData = "FinancialAnalysis.BranchesWithData";
        public const string WithoutData = "FinancialAnalysis.WithoutData";
        public const string WithData = "FinancialAnalysis.WithData";
        public const string BranchDetails = "FinancialAnalysis.BranchDetails";
        public const string SectionSummary = "FinancialAnalysis.SectionSummary";
        public const string IncludedSections = "FinancialAnalysis.IncludedSections";
        public const string Scope = "FinancialAnalysis.Scope";
        public const string Title = "FinancialAnalysis.Title";
        public const string Dashboard = "FinancialAnalysis.Dashboard";
        public const string FinancialPeriod = "FinancialAnalysis.FinancialPeriod";
        public const string PeriodFrom = "FinancialAnalysis.PeriodFrom";
        public const string PeriodTo = "FinancialAnalysis.PeriodTo";
        public const string GeneratedAt = "FinancialAnalysis.GeneratedAt";

        public const string AllBranches = "FinancialAnalysis.AllBranches";
        public const string SelectedBranch = "FinancialAnalysis.SelectedBranch";
        public const string Branch = "FinancialAnalysis.Branch";
        public const string BranchCode = "FinancialAnalysis.BranchCode";
        public const string BranchName = "FinancialAnalysis.BranchName";

        public const string Sales = "FinancialAnalysis.Sales";
        public const string Inventory = "FinancialAnalysis.Inventory";
        public const string Expenses = "FinancialAnalysis.Expenses";
        public const string Purchases = "FinancialAnalysis.Purchases";
        public const string ProfitLoss = "FinancialAnalysis.ProfitLoss";
        public const string CustomersEmployees = "FinancialAnalysis.CustomersEmployees";
        public const string Customers = "FinancialAnalysis.Customers";
        public const string Employees = "FinancialAnalysis.Employees";
        public const string BranchDebt = "FinancialAnalysis.BranchDebt";
        public const string Cash = "FinancialAnalysis.Cash";
        public const string Suppliers = "FinancialAnalysis.Suppliers";
        public const string Capital = "FinancialAnalysis.Capital";
        public const string Transfers = "FinancialAnalysis.Transfers";
        public const string Settlements = "FinancialAnalysis.Settlements";

        public const string Male = "FinancialAnalysis.Male";
        public const string Female = "FinancialAnalysis.Female";

        public const string Total = "FinancialAnalysis.Total";
        public const string Average = "FinancialAnalysis.Average";
        public const string Highest = "FinancialAnalysis.Highest";
        public const string Lowest = "FinancialAnalysis.Lowest";
        public const string Value = "FinancialAnalysis.Value";
        public const string Details = "FinancialAnalysis.Details";

        public const string Debit = "FinancialAnalysis.Debit";
        public const string Credit = "FinancialAnalysis.Credit";
        public const string Balance = "FinancialAnalysis.Balance";
        public const string Opening = "FinancialAnalysis.Opening";
        public const string Closing = "FinancialAnalysis.Closing";
        public const string Change = "FinancialAnalysis.Change";

        public const string Profit = "FinancialAnalysis.Profit";
        public const string Loss = "FinancialAnalysis.Loss";
        public const string BreakEven = "FinancialAnalysis.BreakEven";

        public const string Complete = "FinancialAnalysis.Complete";
        public const string Incomplete = "FinancialAnalysis.Incomplete";
        public const string Missing = "FinancialAnalysis.Missing";
        public const string Valid = "FinancialAnalysis.Valid";
        public const string Invalid = "FinancialAnalysis.Invalid";

        public const string Draft = "FinancialAnalysis.Draft";
        public const string Uploading = "FinancialAnalysis.Uploading";
        public const string Ready = "FinancialAnalysis.Ready";
        public const string Approved = "FinancialAnalysis.Approved";

        public const string ActiveBranches = "FinancialAnalysis.ActiveBranches";
        public const string ExpectedFiles = "FinancialAnalysis.ExpectedFiles";
        public const string ValidFiles = "FinancialAnalysis.ValidFiles";
        public const string MissingFiles = "FinancialAnalysis.MissingFiles";
        public const string CompletionPercentage = "FinancialAnalysis.CompletionPercentage";

        public const string SalesReturns = "FinancialAnalysis.SalesReturns";
        public const string DiscountAllowed = "FinancialAnalysis.DiscountAllowed";
        public const string BranchDiscountAllowed = "FinancialAnalysis.BranchDiscountAllowed";
        public const string NetSales = "FinancialAnalysis.NetSales";

        public const string OpeningInventory = "FinancialAnalysis.OpeningInventory";
        public const string ClosingInventory = "FinancialAnalysis.ClosingInventory";
        public const string InventoryChange = "FinancialAnalysis.InventoryChange";

        public const string TotalExpenses = "FinancialAnalysis.TotalExpenses";
        public const string TotalPurchases = "FinancialAnalysis.TotalPurchases";

        public const string CustomerBalance = "FinancialAnalysis.CustomerBalance";
        public const string EmployeeBalance = "FinancialAnalysis.EmployeeBalance";

        public const string MaleDebt = "FinancialAnalysis.MaleDebt";
        public const string FemaleDebt = "FinancialAnalysis.FemaleDebt";
        public const string TotalDebt = "FinancialAnalysis.TotalDebt";

        public const string TotalCash = "FinancialAnalysis.TotalCash";
        public const string CashboxesCount = "FinancialAnalysis.CashboxesCount";

        public const string SuppliersTotal = "FinancialAnalysis.SuppliersTotal";
        public const string CapitalValue = "FinancialAnalysis.CapitalValue";

        public const string MaleTransfer = "FinancialAnalysis.MaleTransfer";
        public const string FemaleTransfer = "FinancialAnalysis.FemaleTransfer";
        public const string TotalTransfer = "FinancialAnalysis.TotalTransfer";

        public const string PeriodDebit = "FinancialAnalysis.PeriodDebit";
        public const string PeriodCredit = "FinancialAnalysis.PeriodCredit";
        public const string PeriodBalance = "FinancialAnalysis.PeriodBalance";

        public const string ExportExcel = "FinancialAnalysis.ExportExcel";
        public const string ExportPdf = "FinancialAnalysis.ExportPdf";
        public const string Print = "FinancialAnalysis.Print";

        public const string InvalidBranchId = "FinancialAnalysis.InvalidBranchId";
        public const string BranchNotFound = "FinancialAnalysis.BranchNotFound";
        public const string UnsupportedSection = "FinancialAnalysis.UnsupportedSection";
        public const string UnsupportedSections = "FinancialAnalysis.UnsupportedSections";
    }
}
