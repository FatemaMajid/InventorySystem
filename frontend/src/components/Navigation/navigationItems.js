export const navigationGroups = [
  {
    key: "main",
    items: [
      {
        key: "systemHome",
        path: "/modules",
        icon: "home",
        permission: null,
      },
    ],
  },

  {
    key: "inventory",
    items: [
      {
        key: "home",
        path: "/",
        icon: "home",
        permission: "Home.View",
      },
      {
        key: "dashboard",
        path: "/dashboard",
        icon: "dashboard",
        permission: "Dashboard.View",
      },
      {
        key: "inventorySessions",
        path: "/inventory-sessions",
        icon: "inventory",
        permission: "InventorySession.View",
      },
      {
        key: "newInventorySession",
        path: "/new-inventory-session",
        icon: "plus",
        permission: "InventorySession.Create",
      },
      {
        key: "attentionItems",
        path: "/attention-items",
        icon: "attention",
        permission: "Attention.View",
      },
      {
        key: "reports",
        path: "/reports",
        icon: "reports",
        permission: "Report.View",
      },
    ],
  },

  {
    key: "financialAnalysis",
    items: [
      {
        key: "financialHome",
        path: "/financial-analysis",
        icon: "home",
        permission: "FinancialAnalysis.View",
      },
      {
        key: "financialDashboard",
        path: "/financial-analysis/dashboard",
        icon: "dashboard",
        permission: "FinancialDashboard.View",
      },
    ],
  },

  {
    key: "financialData",
    items: [
      {
        key: "financialPeriods",
        path: "/financial-analysis/periods",
        icon: "reports",
        permission: "FinancialPeriod.View",
      },
      {
        key: "financialUploads",
        path: "/financial-analysis/uploads",
        icon: "plus",
        permission: "FinancialPeriod.View",
      },
    ],
  },

  {
    key: "incomeStatement",
    items: [
      {
        key: "financialSales",
        path: "/financial-analysis/sales",
        icon: "reports",
        permission: "FinancialSales.View",
      },
      {
        key: "financialInventory",
        path: "/financial-analysis/inventory",
        icon: "inventory",
        permission: "FinancialInventory.View",
      },
      {
        key: "financialExpenses",
        path: "/financial-analysis/expenses",
        icon: "reports",
        permission: "FinancialExpenses.View",
      },
      {
        key: "financialPurchases",
        path: "/financial-analysis/purchases",
        icon: "reports",
        permission: "FinancialPurchases.View",
      },
      {
        key: "financialProfitLoss",
        path: "/financial-analysis/profit-loss",
        icon: "dashboard",
        permission: "FinancialProfitLoss.View",
      },
    ],
  },

  {
    key: "financialPosition",
    items: [
      {
        key: "financialCustomers",
        path: "/financial-analysis/customers-employees",
        icon: "users",
        permission: "FinancialCustomers.View",
      },
      {
        key: "financialDebt",
        path: "/financial-analysis/debt",
        icon: "reports",
        permission: "FinancialDebt.View",
      },
      {
        key: "financialCash",
        path: "/financial-analysis/cash",
        icon: "dashboard",
        permission: "FinancialCash.View",
      },
      {
        key: "financialSuppliers",
        path: "/financial-analysis/suppliers",
        icon: "users",
        permission: "FinancialSuppliers.View",
      },
      {
        key: "financialCapital",
        path: "/financial-analysis/capital",
        icon: "dashboard",
        permission: "FinancialCapital.View",
      },
    ],
  },

  {
    key: "financialMovements",
    items: [
      {
        key: "financialTransfers",
        path: "/financial-analysis/transfers",
        icon: "reports",
        permission: "FinancialTransfers.View",
      },
      {
        key: "financialSettlements",
        path: "/financial-analysis/settlements",
        icon: "reports",
        permission: "FinancialSettlements.View",
      },
    ],
  },

  {
    key: "masterData",
    items: [
      {
        key: "branches",
        path: "/branches",
        icon: "branches",
        permission: "Branch.View",
      },
      {
        key: "stores",
        path: "/stores",
        icon: "stores",
        permission: "Store.View",
      },
    ],
  },

  {
    key: "administration",
    items: [
      {
        key: "users",
        path: "/users",
        icon: "users",
        permission: "User.View",
      },
      {
        key: "rolesPermissions",
        path: "/roles-permissions",
        icon: "roles",
        permission: "Role.View",
      },
      {
        key: "auditLogs",
        path: "/audit-logs",
        icon: "audit",
        permission: "AuditLog.View",
      },
    ],
  },
];