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
    sessionInformation: 'Inventory Session Information',
    sessionInformationDescription:
      'Enter the basic information for the new inventory session.',

    inventoryType: 'Inventory Type',
    selectInventoryType: 'Select inventory type',

    inventoryTypes: {
      semiAnnual: 'Semi-Annual Inventory',
      annual: 'Annual Inventory',
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
  },
};

export default en;