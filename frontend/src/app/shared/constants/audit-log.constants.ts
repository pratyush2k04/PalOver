export const AUDIT_LOG_ROUTES = {
  BASE: 'audit-log',
} as const;

export const AUDIT_LOG_API_ENDPOINTS = {
  LIST: '/api/audit-logs',
} as const;

export const AUDIT_LOG_MESSAGES = {
  LOAD_ERROR: 'Unable to load the audit log. Please try again.',
  UNAUTHORIZED_ERROR: 'Unauthorized. Please log in again to view the audit log.',
} as const;
