export const PORTFOLIO_STATUS = {
  ALL: 'ALL',
  KICKOFF_READY: 'Kickoff-ready',
  IN_PROGRESS: 'In progress',
  AT_RISK: 'At risk',
} as const;

export const PORTFOLIO_DAYS_FILTER = {
  ALL: 'ALL',
  DAYS_7: '7',
  DAYS_14: '14',
  DAYS_30: '30',
} as const;

export const PORTFOLIO_MESSAGES = {
  LOAD_ERROR: 'Failed to load portfolio dashboard data. Please try again.',
  OVERDUE_PREFIX: 'Overdue',
  TODAY: 'Today',
  DAYS_SUFFIX: 'days',
  NO_ENGAGEMENTS_DESC: 'No active engagements match your selected filter criteria.',
} as const;

export const PORTFOLIO_BADGE_CLASSES = {
  READINESS_SUCCESS: 'readiness--success',
  READINESS_INFO: 'readiness--info',
  READINESS_WARNING: 'readiness--warning',
  READINESS_DANGER: 'readiness--danger',
} as const;
