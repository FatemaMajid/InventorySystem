const en = {
  common: {
    systemName: 'Inventory Management System',
    user: 'User',
    inventoryUser: 'Inventory User',
    logout: 'Logout',
    notifications: 'Notifications',
    lightMode: 'Light Mode',
    darkMode: 'Dark Mode',
  },

  navigation: {
    home: 'Home',
    dashboard: 'Dashboard',
    inventorySessions: 'Inventory Sessions',
    newInventorySession: 'New Inventory Session',
    comparisonResults: 'Comparison Results',
    attentionItems: 'Attention Items',
    reports: 'Reports',
    branches: 'Branches',
    stores: 'Stores',
    users: 'Users',
    rolesPermissions: 'Roles & Permissions',
    auditLogs: 'Audit Logs',
    settings: 'Settings',
  },

  navigationGroups: {
    main: 'MAIN',
    inventory: 'INVENTORY',
    results: 'RESULTS',
    masterData: 'MASTER DATA',
    administration: 'ADMINISTRATION',
    system: 'SYSTEM',
  },

  inventory: {

    inventoryType: 'Inventory Type',

    selectInventoryType:
      'Select inventory type',

    inventoryTypes: {
      semiAnnual: 'Semi-Annual Inventory',
      annual: 'Annual Inventory',
    },

    beforeInventory: 'Before Inventory',

    beforeInventoryDescription:
      'Upload the Excel file before starting the inventory.',

    afterInventory: 'After Inventory',

    afterInventoryDescription:
      'Upload the Excel file after completing the inventory.',

    invalidExcelFile:
      'Only Excel files are allowed (.xlsx or .xls).',

    removeFile: 'Remove file',

    checkingFile:
      'Checking file...',

    fileValidated:
      'File validated successfully',

    totalRows: 'Total Rows',

    validRows: 'Valid Rows',

    errorRows: 'Error Rows',

    previewError:
      'Failed to validate the Excel file.',

    createSessionError:
      'Failed to create the inventory session.',

    creatingSession:
      'Creating Session...',

    sessionInformation: 'Inventory Session Information',
    sessionInformationDescription:
      'Enter the basic information for the new inventory session.',

    inventoryType: 'Inventory Type',
    selectInventoryType: 'Select inventory type',

    inventoryTypes: {
      semiAnnual: 'Semi-Annual Inventory',
      annual: 'Annual Inventory',
    },

    requiredColumns: 'Required Columns',
    requiredColumnsDescription:
      'Both Excel files must contain the following columns.',

    requiredColumnNames: {
      itemCode: 'Item Code',
      itemName: 'Item Name',
      category: 'Category',
      quantity: 'Quantity',
      price: 'Price',
    },

    selectBranch: 'Select Branch',
    selectStore: 'Select Store',

    inventoryDate: 'Inventory Date',

    notes: 'Notes',
    notesPlaceholder:
      'Add any notes about this inventory session.',

    uploadFile: 'Upload Inventory Files',
    uploadFileDescription:
      'Upload the Excel files required for the inventory session.',

    beforeInventory: 'Before Inventory',
    beforeInventoryDescription:
      'Upload the Excel file before starting the inventory.',

    afterInventory: 'After Inventory',
    afterInventoryDescription:
      'Upload the Excel file after completing the inventory.',

    chooseFile: 'Choose Excel File',
    dropFile: 'Drop your Excel file here',
    or: 'or',

    requiredColumns: 'Required Columns',
    requiredColumnsDescription:
      'Both Excel files must contain the following columns.',

    createSession: 'Create Session',
    cancel: 'Cancel',
    invalidExcelFile: 'Only Excel files are allowed (.xlsx or .xls).',
  },

  home: {
    welcomeLabel: 'Welcome back',
    title: 'Inventory Management System',
    subtitle:
      'Manage inventory sessions, review results, and monitor your inventory operations.',

    quickActions: 'Quick Actions',
    quickActionsSubtitle:
      'Access the most frequently used inventory operations.',

    newInventory: 'New Inventory Session',
    newInventoryDescription:
      'Start a new inventory counting session.',

    inventorySessions: 'Inventory Sessions',
    inventorySessionsDescription:
      'View and manage inventory sessions.',

    reports: 'Reports',
    reportsDescription:
      'Review and export inventory reports.',

    overview: 'Overview',
    overviewSubtitle:
      'A quick overview of your inventory system.',

    activeSessions: 'Active Sessions',
    branches: 'Branches',
    attentionItems: 'Attention Items',

    activeSessionsDescription: 'Currently in progress',

    totalSessionsDescription: 'All inventory sessions',

    attentionItemsDescription: 'Require review',
    totalSessions: 'Total Sessions',

    recentSessions: {
      title: "Recent Inventory Sessions",
      viewAll: "View All Sessions",


      sessionId: "Session ID",
      date: "Date",
      branch: "Branch",
      store: "Store",
      status: "Status",
      items: "Items",
      actions: "Actions",
      view: "View",
      noSessions: "No inventory sessions",

      statuses: {
        completed: "Completed",
        inProgress: "In Progress",
        cancelled: "Cancelled",
      },
    },
    itemCategories: "Item Categories",
    stores: "stores",
    totalBranches: "Total branches",
    totalStores: "Total stores",
    totalCategories: "Total categories",
    totalItems: "Total items",

    systemStatus: 'System Status',
    apiStatus: 'API Status',
    database: 'Database',
    lastBackup: 'Last Backup',
    systemHealth: 'System Health',
    connected: 'Connected',
    healthy: 'Healthy',
    lastBackupValue: 'Today, 03:00 PM',
  },

  dashboard: {
    header: {
      title: 'Inventory Dashboard',
      session: 'Session',
      status: 'Status',
      type: 'Type',
      date: 'Date',
      branch: 'Branch',
      store: 'Store',

      exportExcel: 'Export Excel',
      exportPdf: 'Export PDF',

      statuses: {
        completed: 'Completed',
      },

      inventoryTypes: {
        semiAnnual: 'Semi-Annual Inventory',
        annual: 'Annual Inventory',
      },
    },

    kpiCards: {
      totalItems: 'Total Items',
      increase: 'Increase',
      decrease: 'Decrease',
      match: 'Match',
      newlyCounted: 'Newly Counted',
      fullyDepleted: 'Fully Depleted',
      priceChanged: 'Price Changed',
      unitNotDefined: 'unit Not Defined',
      title: 'Item Status',
      total: 'Total',

      topDifferences: {
        title: 'TOP 10 BY VALUE DIFFERENCE (IQD)'
      },
    },

    financialSummary: {
      title: "Financial Summary",
      totalValueBefore: "Total Value Before",
      totalValueAfter: "Total Value After",
      totalDifference: "Total Difference",
      differencePercentage: "Difference %",
      currency: 'IQD',
    },

    valueChart: {
      title: "Inventory Value",
      subtitle: "Comparison of inventory value before and after the session.",
      before: "Before Inventory",
      after: "After Inventory",
    },

    attention: {
      title: 'Attention Required',
      description: 'items require your attention',
      viewAll: 'View All',
      view: 'View',
      newlyCounted: 'Newly Counted',
      fullyDepleted: 'Fully Depleted',
      unitNotDefined: 'Unit Not Defined',
      priceChanged: 'Price Changed',
    },

    comparisonResults: {
      title: "Comparison Results",
      items: "items",

      filters: "Filters",
      clear: "Clear",
      status: "Status",
      search: "Search (Item Code / Name)",
      searchPlaceholder: "Search...",
      category: "Category",
      unit: "Unit",
      all: "All",
      applyFilters: "Apply Filters",

      itemCode: "Item Code",
      itemName: "Item Name",
      quantityBefore: "Quantity Before",
      quantityAfter: "Quantity After",
      quantityDifference: "Quantity Difference",
      differencePercentage: "Difference %",
      priceBefore: "Price Before",
      priceAfter: "Price After",
      beforeValue: "Value Before",
      afterValue: "Value After",
      valueDifference: "Value Difference",
      status: "Status",

      loading: "Loading...",
      noResults: "No results found",

      of: "of",
      previous: "Previous",
      next: "Next",

      statuses: {
        increase: "Increase",
        decrease: "Decrease",
        noDifference: "No Difference",
        afterOnly: "After Inventory Only",
        beforeOnly: "Before Inventory Only",
      },
    },
  },
  footer: {
    systemName: 'Inventory Management System',
    systemDescription: 'Smart • Reliable • Efficient',
    copyright: 'Fatema Majid 2026',
    version: 'v1.0.0',
    allRightsReserved: 'All rights reserved',
  },

}

export default en;