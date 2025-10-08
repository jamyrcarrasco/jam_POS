export const APP_CONSTANTS = {
  STORAGE_KEYS: {
    TOKEN: 'token',
    USER: 'user',
    THEME: 'theme',
    LANGUAGE: 'language'
  },
  ROLES: {
    SUPER_ADMIN: 'SuperAdmin',
    ADMIN: 'Admin',
    SELLER: 'Seller',
    CASHIER: 'Cashier'
  },
  PAGINATION: {
    DEFAULT_PAGE_SIZE: 10,
    PAGE_SIZE_OPTIONS: [5, 10, 25, 50, 100]
  },
  VALIDATION: {
    PASSWORD_MIN_LENGTH: 6,
    USERNAME_MIN_LENGTH: 3
  }
} as const;
