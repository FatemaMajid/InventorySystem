const en = {
  common: {
    systemName: 'Inventory Management System',
    user: 'User',
    inventoryUser: 'Inventory User',
    logout: 'Logout',
    notifications: 'Notifications',
    lightMode: 'Light Mode',
    darkMode: 'Dark Mode',
    loading: "Loading...",
    noResults: "No results found.",
    error: "Something went wrong.",
    retry: "Retry",
    changeLanguageToArabic: "Change language to Arabic",
    changeLanguageToEnglish: "Change language to English",
    english: "English",
    arabic: "Arabic",
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
      total: "Total Attention",
      session: "Inventory Session",
      selectSession: "Select Session",
      attentionType: "Attention Type",
    },

    comparisonResults: {
      description: "Comparison between the previous and current inventory",
      totalItems: "Total Items",
      branch: "Branch",
      store: "Store",
      inventoryType: "Inventory Type",
      loadError: "Failed to load comparison results",
      noActiveSession: "No active inventory session",
      title: "Comparison Results",
      items: "items",

      filters: "Filters",
      clear: "Clear",
      status: "Status",
      search: "Search (Item Code)",
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

  inventorySessions: {
    title: "Inventory Sessions",
    description: "View and manage inventory sessions.",

    totalSessions: "Total Sessions",
    activeSessions: "Active Sessions",
    completedSessions: "Completed Sessions",

    searchPlaceholder: "Search sessions...",

    status: "Status",
    branch: "Branch",
    store: "Store",

    allStatuses: "All Statuses",
    allBranches: "All Branches",
    allStores: "All Stores",

    active: "Active",
    completed: "Completed",
    inProgress: "In Progress",
    cancelled: "Cancelled",

    clearFilters: "Clear",

    sessionsList: "Inventory Sessions",
    sessionsListDescription: "Recent inventory sessions.",

    sessionId: "Session ID",
    date: "Date",
    items: "Items",
    actions: "Actions",

    view: "View",
    loading: "Loading...",
    noSessions: "No inventory sessions.",
    loadError: "Failed to load inventory sessions.",
    noOptions: "No options",
  },
  stores: {
    description: "Manage stores, branches and store status.",
    newStore: "Add Store",
    editStore: "Edit Store",
    totalStores: "Total Stores",
    activeStores: "Active Stores",
    inactiveStores: "Inactive Stores",
    searchPlaceholder: "Search by code, name or branch...",
    branch: "Branch",
    allBranches: "All Branches",
    status: "Status",
    allStatuses: "All Statuses",
    active: "Active",
    inactive: "Inactive",
    reset: "Reset",
    storesList: "Stores List",
    storesListDescription: "View and manage all stores.",
    code: "Store Code",
    name: "Store Name",
    actions: "Actions",
    edit: "Edit",
    delete: "Delete",
    noStores: "No stores found.",
    loading: "Loading stores...",
    loadError: "Failed to load stores.",
    saveError: "Failed to save store.",
    deleteError: "Failed to delete store.",
    deleteConfirmation: "Are you sure you want to delete {name}?",
    editDescription: "Update the store information below.",
    arabicName: "Arabic Name",
    englishName: "English Name",
    selectBranch: "Select Branch",
    formDescription: "Enter the store information below.",
    storeCode: "Store Code",
    storeNameArabic: "Arabic Name",
    storeNameEnglish: "English Name",
    branchCode: "Branch",
    close: "Close",
    cancel: "Cancel",
    save: "Save",
    saving: "Saving...",
  },
  pagination: {
    showing: "Showing",
    of: "of",
    rowsPerPage: "Rows per page",
    previous: "Previous",
    next: "Next",
  },

  reports: {
    title: "Inventory Report",
    description: "Review, analyze and export results for the selected inventory session.",
    session: "Inventory Session",
    date: "Inventory Date",
    branch: "Branch",
    store: "Store",
    noActiveSession: "Select an inventory session to view its report.",
    error: "An error occurred while loading the report.",
    exportError: "An error occurred while exporting the report.",

    summary: {
      totalItems: "Total Items",
      match: "Matched Items",
      differences: "Items with Differences",
      attention: "Attention Items",
    },

    financial: {
      before: "Value Before",
      after: "Value After",
      difference: "Value Difference",
      percentage: "Difference %",
    },

    tabs: {
      overview: "Overview",
      comparison: "Comparison",
    },

    actions: {
      excel: "Export Excel",
      pdf: "Export PDF",
      print: "Print",
    },

    statusOverview: {
      title: "Inventory Status",
      description: "Distribution of counted inventory results.",
    },

    statuses: {
      Increase: "Increase",
      Decrease: "Decrease",
      Match: "Matched",
      NewlyCounted: "Newly Counted",
      FullyDepleted: "Fully Depleted",
    },

    topDifferences: {
      title: "Top Value Differences",
      description: "Items with the largest value changes.",
      empty: "No value differences found.",
    },
  },

  attentionItems: {
    title: "Attention Items",
    description: "Items that require review or follow-up.",
    session: "Inventory Session",
    sessionDate: "Inventory Date",
    branch: "Branch",
    store: "Store",
    inventoryType: "Inventory Type",

    summary: {
      total: "Total Attention Items",
      newlyCounted: "Newly Counted",
      fullyDepleted: "Fully Depleted",
      unitNotDefined: "Unit Not Defined",
      priceChanged: "Price Changed",
    },

    filters: {
      search: "Search by item code",
      attentionType: "Attention Type",
      category: "Category",
      unit: "Unit",
      all: "All",
      clear: "Clear",
    },

    types: {
      newlyCounted: "Newly Counted",
      fullyDepleted: "Fully Depleted",
      unitNotDefined: "Unit Not Defined",
      priceChanged: "Price Changed",
    },

    table: {
      itemCode: "Item Code",
      itemName: "Item Name",
      category: "Category",
      unit: "Unit",
      quantityBefore: "Previous Quantity",
      quantityAfter: "Current Quantity",
      difference: "Difference",
      consumerPriceBefore: "Previous Consumer Price",
      consumerPriceAfter: "Current Consumer Price",
      valueBefore: "Previous Value",
      valueAfter: "Current Value",
      valueDifference: "Value Difference",
      attentionType: "Attention Type",
    },

    empty: "No attention items found.",
    loading: "Loading data...",
    error: "An error occurred while loading attention items.",
  },

  branches: {
    description: "Manage branches and their contact information.",
    addBranch: "Add Branch",
    editBranch: "Edit Branch",
    totalBranches: "Total Branches",
    activeBranches: "Active Branches",
    inactiveBranches: "Inactive Branches",
    searchPlaceholder: "Search by code or branch name...",
    status: "Status",
    allStatuses: "All Statuses",
    active: "Active",
    inactive: "Inactive",
    clear: "Clear",
    listTitle: "Branches List",
    listDescription: "View and manage all branches.",
    code: "Branch Code",
    arabicName: "Arabic Name",
    englishName: "English Name",
    address: "Address",
    phone: "Phone",
    actions: "Actions",
    edit: "Edit",
    delete: "Delete",
    empty: "No branches found.",
    loading: "Loading branches...",
    loadError: "Failed to load branches.",
    loadDetailsError: "Failed to load branch details.",
    saveError: "Failed to save branch.",
    deleteError: "Failed to delete branch.",
    deleteConfirmation: "Are you sure you want to delete {name}?",
    formDescription: "Enter the branch information below.",
    close: "Close",
    cancel: "Cancel",
    save: "Save",
    saving: "Saving...",
  },

  users: {
    title: "Users",
    description: "Manage users and permissions",
    addUser: "Add User",
    editUser: "Edit User",
    addUserDescription: "Create a new user and assign permissions",
    editUserDescription: "Edit user information and permissions",
    username: "Username",
    usernamePlaceholder: "Enter username",
    password: "Password",
    passwordPlaceholder: "Enter password",
    passwordEditPlaceholder: "Leave empty to keep the current password",
    role: "Role",
    selectRole: "Select role",
    activeAccount: "Active account",
    permissions: "Permissions",
    permissionsDescription: "Select direct permissions for this user",
    save: "Save",
    saving: "Saving...",
    cancel: "Cancel",
    close: "Close",
    edit: "Edit",
    delete: "Delete",
    deactivate: "Deactivate",
    activate: "Activate",
    active: "Active",
    inactive: "Inactive",
    totalUsers: "Total Users",
    activeUsers: "Active Users",
    inactiveUsers: "Inactive Users",
    searchPlaceholder: "Search users...",
    allStatuses: "All Statuses",
    noUsers: "No users found",
    loading: "Loading users...",
    actions: "Actions",
    confirmDelete: "Are you sure you want to delete this user?",
    confirmDeactivate: "Are you sure you want to deactivate this user?",
    confirmActivate: "Do you want to activate this user?",
    createdAt: "Created At",
    updatedAt: "Last Updated",
    permissionsCount: "Permissions",
    successCreate: "User created successfully",
    successUpdate: "User updated successfully",
    successDelete: "User deleted successfully",
    errorLoad: "Failed to load users",
    errorSave: "Failed to save user",
    errorDelete: "Failed to delete user",
    status: "Status",
    notAvailable: "Not Available",
    allRoles: "All Roles",
  },
  login: {
    title: "Inventory Management System",
    description: "Sign in to continue",
    username: "Username",
    usernamePlaceholder: "Enter your username",
    password: "Password",
    passwordPlaceholder: "Enter your password",
    signIn: "Sign In",
    signingIn: "Signing in...",
    requiredFields: "Please enter username and password.",
    loginError: "Invalid username or password.",
  },

  rolesPermissions: {
    title: "Roles & Permissions",
    description: "Manage user roles and their associated permissions.",
    addRole: "Add Role",
    editRole: "Edit Role",
    roles: "Roles",
    rolesDescription: "View and manage system roles.",
    totalRoles: "Total Roles",
    totalPermissions: "Total Permissions",
    permissions: "Permissions",
    permissionsDescription: "Permissions assigned to this role.",
    users: "Users",
    selectRoleTitle: "Select a Role",
    selectRoleDescription: "Select a role from the list to view its assigned permissions.",
    roleName: "Role Name",
    roleDescription: "Role Description",
    formDescription: "Enter the role information and select the required permissions.",
    close: "Close",
    cancel: "Cancel",
    save: "Save",
    saving: "Saving...",
    retry: "Retry",
    loadError: "Failed to load roles and permissions.",
    loadDetailsError: "Failed to load role details.",
    saveError: "Failed to save role.",
    permissionGroups: {
      Dashboard: "Dashboard",
      InventorySession: "Inventory Sessions",
      Attention: "Attention Items",
      Report: "Reports",
      Branch: "Branches",
      Store: "Stores",
      User: "Users",
      Role: "Roles & Permissions",
      AuditLog: "Audit Log",
      Settings: "Settings",
    },
    permissionNames: {
      "Dashboard.View": "View Dashboard",
      "Dashboard.Export": "Export Dashboard",
      "InventorySession.View": "View Inventory Sessions",
      "InventorySession.Create": "Create Inventory Session",
      "Attention.View": "View Attention Items",
      "Report.View": "View Reports",
      "Branch.View": "View Branches",
      "Branch.Create": "Create Branch",
      "Branch.Edit": "Edit Branch",
      "Store.View": "View Stores",
      "Store.Create": "Create Store",
      "Store.Edit": "Edit Store",
      "User.View": "View Users",
      "User.Create": "Create User",
      "User.Edit": "Edit User",
      "User.Deactivate": "Deactivate User",
      "Role.View": "View Roles & Permissions",
      "Role.Edit": "Edit Roles & Permissions",
      "AuditLog.View": "View Audit Log",
      "Settings.View": "View Settings",
    },
    roleNames: {
      Manager: "Manager",
      Admin: "Administrator",
      User: "User",
    },
    roleDescriptions: {
      Manager: "Full system access.",
      Admin: "Inventory and user administration.",
      User: "View-only access.",
    },
  },

  auditLog: {
    title: "Audit Log",
    description: "View and monitor all operations performed in the system.",
    stats: {
      total: "Total Operations",
      today: "Today's Operations",
      success: "Successful Operations",
      failed: "Failed Operations"
    },
    filters: {
      title: "Filter Operations",
      reset: "Reset",
      user: "User",
      action: "Action",
      entity: "Entity",
      dateFrom: "From Date",
      dateTo: "To Date",
      all: "All"
    },
    table: {
      title: "Audit Log",
      date: "Date",
      user: "User",
      action: "Action",
      entity: "Entity",
      status: "Status",
      details: "Details",
      empty: "No operations found",
      loading: "Loading..."
    },
    details: {
      title: "Operation Details",
      close: "Close",
      entityId: "Entity ID",
      details: "Details",
      name: "Name",
      code: "Code",
      branchCode: "Branch Code"
    },
    actions: {
      Create: "Create",
      Update: "Update",
      Delete: "Delete",
      Confirm: "Confirm",
      Login: "Login",
      LoginFailed: "Login Failed"
    },
    entities: {
      Branch: "Branch",
      Store: "Store",
      User: "User",
      Role: "Role",
      InventorySession: "Inventory Session"
    },
    status: {
      Success: "Successful",
      Failed: "Failed"
    }
  },
};

export default en;
