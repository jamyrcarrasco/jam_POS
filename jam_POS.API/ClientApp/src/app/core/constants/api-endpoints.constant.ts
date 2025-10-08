export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: 'auth/login',
    REGISTER: 'auth/register',
    REFRESH: 'auth/refresh',
    LOGOUT: 'auth/logout',
    PROFILE: 'auth/profile'
  },
  PRODUCTS: {
    BASE: 'products',
    SEARCH: 'products/search',
    CATEGORIES: 'products/categories'
  },
  SALES: {
    BASE: 'sales',
    REPORTS: 'sales/reports'
  },
  CUSTOMERS: {
    BASE: 'customers'
  },
  DASHBOARD: {
    STATS: 'dashboard/stats',
    RECENT_SALES: 'dashboard/recent-sales'
  }
} as const;
