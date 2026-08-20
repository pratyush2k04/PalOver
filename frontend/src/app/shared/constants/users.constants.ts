export const APP_ROUTES = {
  LOGIN: 'login',
  DASHBOARD: 'dashboard',
  USERS: 'admin/users',
  ENGAGEMENTS: 'engagements',
  MY_ENGAGEMENTS: 'my-engagements',
  MY_HANDOVERS: 'my-handovers',
  TECHNICAL_READINESS: 'technical-readiness',
  PORTFOLIO: 'portfolio',
  PORTFOLIO_DASHBOARD: 'portfolio-dashboard',
  USER_DIRECTORY: 'user-directory',
  ENGAGEMENT_ALLOCATION: 'engagement-allocation',
  ACCESS_DENIED: 'access-denied',
  NOTIFICATIONS: 'notifications',
  AUDIT_LOG: 'audit-log',
  PROFILE: 'profile',
} as const;

export const API_ENDPOINTS = {
  USERS: '/api/users',
  ROLES: '/api/roles',
  USER_ROLE: (userId: number) => `/api/users/${userId}/role`,
  ROLE_USERS: (roleId: number) => `/api/roles/${roleId}/users`,
} as const;

export const USER_ROLES = {
  ADMIN: 'Admin',
  PRE_SALES: 'Pre Sales',
  TECH_LEAD: 'Tech Lead',
  PROJECT_MANAGEMENT: 'Project Management',
  LEADERSHIP: 'Leadership',
} as const;

export const USER_MESSAGES = {
  LOAD_USERS_ERROR: 'Unable to load users. Please try again.',
  LOAD_ROLES_ERROR: 'Unable to load roles. Please try again.',
  UPDATE_ROLE_ERROR: 'Unable to update the role. Please try again.',
  ROLE_UPDATED_SUCCESS: 'Role updated',
} as const;
