export const APP_ROUTES = {
  LOGIN: '/login',
  DASHBOARD: '/dashboard',
  USERS: '/admin/users',
  ENGAGEMENTS: '/engagements',
  MY_ENGAGEMENTS: '/my-engagements',
  MY_HANDOVERS: '/my-handovers',
  TECHNICAL_READINESS: '/technical-readiness',
  PORTFOLIO: '/portfolio',
  PORTFOLIO_DASHBOARD: '/portfolio-dashboard',
  USER_DIRECTORY: '/user-directory',
  ENGAGEMENT_ALLOCATION: '/engagement-allocation',
  ACCESS_DENIED: '/access-denied',
  NOTIFICATIONS: '/notifications',
  AUDIT_LOG: '/audit-log',
  MASTER_CHECKLIST: '/master-checklist',
  PROFILE: '/profile',
} as const;

export const AUTH_ROUTES = {
  LOGIN: 'login',
} as const;

export const PROFILE_ROUTES = {
  BASE: 'profile',
} as const;

export const ACCESS_DENIED_ROUTES = {
  BASE: 'access-denied',
} as const;

export const USERS_ROUTES = {
  BASE: 'admin/users',
} as const;

export const ENGAGEMENT_ALLOCATION_ROUTES = {
  BASE: 'engagement-allocation',
} as const;

export const PORTFOLIO_ROUTES = {
  BASE: 'portfolio',
} as const;

export const NOTIFICATION_ROUTES = {
  BASE: 'notifications',
} as const;

export const ENGAGEMENT_ROUTES = {
  BASE: 'engagements',
  MY_ENGAGEMENTS: 'my-engagements',
  WORKSPACE: 'workspace',
  OVERVIEW: 'overview',
  HANDOVER: 'handover',
  DOCUMENTS: 'documents',
  PEOPLE: 'people',
  CHECKLIST: 'checklist',
  KICKOFF: 'kickoff',
  RESOURCES: 'resources',
  TECHNICAL_READINESS: 'technical-readiness',
  RISKS: 'risks',
  ASSUMPTIONS: 'assumptions-and-decisions',
  AUDITS: 'audits',
  DAY_ONE_SUMMARY_PRINT: 'day-one-summary-print',
} as const;

export const MASTER_CHECKLIST_ROUTES = {
  BASE: 'master-checklist',
  PUBLIC_BASE: 'checklist',
  DRAFT: 'draft',
  PUBLISHED: 'published',
  HISTORY: 'history',
} as const;

export const TECHNICAL_READINESS_ROUTES = {
  BASE: 'technical-readiness',
} as const;

export const RISK_ROUTES = {
  BASE: 'risks',
} as const;

export const HOME_ROUTES_CONFIG = {
  USER_DIRECTORY: 'user-directory',
  ENGAGEMENT_DASHBOARD: 'engagement-dashboard',
  MY_HANDOVERS: 'my-handovers',
  TECHNICAL_READINESS: 'technical-readiness',
  PORTFOLIO_DASHBOARD: 'portfolio-dashboard',
} as const;

export const AUDIT_LOG_ROUTES = {
  BASE: 'audit-log',
} as const;
