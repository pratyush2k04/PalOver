export const AUDIT_TRAIL_ROUTES = {
  BASE: 'audit-trail',
} as const;

export const AUDIT_LOG_API_ENDPOINTS = {
  LIST: '/api/audit-logs',
} as const;

export const AUDIT_LOG_MESSAGES = {
  LOAD_ERROR: 'Unable to load the audit trail. Please try again.',
} as const;
