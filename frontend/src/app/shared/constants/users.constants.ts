export const API_ENDPOINTS = {
  USERS: '/api/users',
  ROLES: '/api/roles',
  USER_ROLE: (userId: number) => `/api/users/${userId}/role`,
  ROLE_USERS: (roleId: number) => `/api/roles/${roleId}/users`,
} as const;

export enum UserRoleId {
  PreSales = 1,
  Leadership = 2,
  Delivery = 3,
}

export enum EngagementRoleId {
  PreSales = 1,
  Leadership = 2,
  ProjectManager = 3,
  TechLead = 4,
}

export const USER_ROLES = {
  ADMIN: 'Admin',
  PRE_SALES: 'Pre Sales',
  LEADERSHIP: 'Leadership',
  DELIVERY: 'Delivery',
} as const;

export const ENGAGEMENT_ROLES = {
  PRE_SALES: 'Pre Sales',
  LEADERSHIP: 'Leadership',
  PROJECT_MANAGER: 'Project Manager',
  TECH_LEAD: 'Tech Lead',
} as const;

export const USER_MESSAGES = {
  LOAD_USERS_ERROR: 'Unable to load users. Please try again.',
  LOAD_ROLES_ERROR: 'Unable to load roles. Please try again.',
  UPDATE_ROLE_ERROR: 'Unable to update the role. Please try again.',
  ROLE_UPDATED_SUCCESS: 'Role updated',
} as const;
