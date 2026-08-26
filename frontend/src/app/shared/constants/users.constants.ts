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
