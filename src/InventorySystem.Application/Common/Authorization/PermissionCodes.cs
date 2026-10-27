namespace InventorySystem.Application.Common.Authorization;

public static class PermissionCodes
{
    public const string DashboardView = "Dashboard.View";
    public const string DashboardExport = "Dashboard.Export";

    public const string HomeView = "Home.View";

    public const string InventorySessionView = "InventorySession.View";
    public const string InventorySessionCreate = "InventorySession.Create";

    public const string ComparisonView = "Comparison.View";
    public const string ComparisonExport = "Comparison.Export";

    public const string AttentionView = "Attention.View";
    public const string AttentionExport = "Attention.Export";

    public const string ReportView = "Report.View";
    public const string ReportCreate = "Report.Create";
    public const string ReportExport = "Report.Export";

    public const string BranchView = "Branch.View";
    public const string BranchCreate = "Branch.Create";
    public const string BranchEdit = "Branch.Edit";

    public const string StoreView = "Store.View";
    public const string StoreCreate = "Store.Create";
    public const string StoreEdit = "Store.Edit";

    public const string UserView = "User.View";
    public const string UserCreate = "User.Create";
    public const string UserEdit = "User.Edit";
    public const string UserDeactivate = "User.Deactivate";

    public const string RoleView = "Role.View";
    public const string RoleEdit = "Role.Edit";

    public const string AuditLogView = "AuditLog.View";

    public const string SettingsView = "Settings.View";

    public const string FinancialPeriodView = "FinancialPeriod.View";
    public const string FinancialPeriodCreate = "FinancialPeriod.Create";
    public const string FinancialPeriodApprove = "FinancialPeriod.Approve";

    // ==========================================
    // Financial Analysis
    // ==========================================

    public const string FinancialAnalysisView =
        "FinancialAnalysis.View";

    public const string FinancialSalesView =
        "FinancialSales.View";

    public const string FinancialInventoryView =
        "FinancialInventory.View";

    public const string FinancialExpensesView =
        "FinancialExpenses.View";

    public const string FinancialPurchasesView =
        "FinancialPurchases.View";

    public const string FinancialProfitLossView =
        "FinancialProfitLoss.View";

    public const string FinancialCustomersView =
        "FinancialCustomers.View";

    public const string FinancialDebtView =
        "FinancialDebt.View";

    public const string FinancialCashView =
        "FinancialCash.View";

    public const string FinancialSuppliersView =
        "FinancialSuppliers.View";

    public const string FinancialCapitalView =
        "FinancialCapital.View";

    public const string FinancialTransfersView =
        "FinancialTransfers.View";

    public const string FinancialSettlementsView =
        "FinancialSettlements.View";

    public const string FinancialDashboardView =
        "FinancialDashboard.View";
}
