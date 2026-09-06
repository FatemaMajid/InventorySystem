export const navigationGroups = [
  {
    key: 'main',
    items: [
      {
        key: 'home',
        path: './',
        icon: 'home',
        permission: 'Home.View',
      },
      
      {
        key: 'dashboard',
        path: '/dashboard',
        icon: 'dashboard',
        permission: 'Dashboard.View',
      },
    ],
  },

  {
    key: 'inventory',
    items: [
      {
        key: 'inventorySessions',
        path: '/inventory-sessions',
        icon: 'inventory',
        permission: 'InventorySession.View',
      },
      {
        key: 'newInventorySession',
        path: '/new-inventory-session',
        icon: 'plus',
        permission: 'InventorySession.Create',
      },
    ],
  },

  {
    key: 'results',
    items: [
      {
        key: 'attentionItems',
        path: '/attention-items',
        icon: 'attention',
        permission: 'Attention.View',
      },
      {
        key: 'reports',
        path: '/reports',
        icon: 'reports',
        permission: 'Report.View',
      },
    ],
  },

  {
    key: 'masterData',
    items: [
      {
        key: 'branches',
        path: '/branches',
        icon: 'branches',
        permission: 'Branch.View',
      },
      {
        key: 'stores',
        path: '/stores',
        icon: 'stores',
        permission: 'Store.View',
      },
    ],
  },

  {
    key: 'administration',
    items: [
      {
        key: 'users',
        path: '/users',
        icon: 'users',
        permission: 'User.View',
      },
      {
        key: 'rolesPermissions',
        path: '/roles-permissions',
        icon: 'roles',
        permission: 'Role.View',
      },
      {
        key: 'auditLogs',
        path: '/audit-logs',
        icon: 'audit',
        permission: 'AuditLog.View',
      },
    ],
  },

  {
    key: 'system',
    items: [
      {
        key: 'settings',
        path: '/settings',
        icon: 'settings',
        permission: 'Settings.View',
      },
    ],
  },
];