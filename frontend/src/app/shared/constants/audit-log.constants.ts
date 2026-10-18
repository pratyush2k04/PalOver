export const AUDIT_LOG_API_ENDPOINTS = {
  LIST: '/api/audit-logs',
  ENGAGEMENT_LIST: '/api/engagementaudits',
} as const;

export const AUDIT_LOG_MESSAGES = {
  LOAD_ERROR: 'Unable to load the audit log. Please try again.',
  UNAUTHORIZED_ERROR: 'Unauthorized. Please log in again to view the audit log.',
} as const;

export const AUDIT_LOG_TOOLTIPS = {
  FILTER_USER: 'Filter audit log by user',
  FILTER_CATEGORY: 'Filter audit log by event category',
  FILTER_FROM_DATE: 'Filter audit log from this date',
  FILTER_TO_DATE: 'Filter audit log up to date',
  EXPORT_CSV: 'Export audit log to CSV',
} as const;

