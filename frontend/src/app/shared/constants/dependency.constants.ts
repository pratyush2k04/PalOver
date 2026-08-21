export const DEPENDENCY_API_ENDPOINTS = {
  GET_DEPENDENCIES: (engagementId: number) => `/api/engagements/${engagementId}/workspace/dependencies`,
  CREATE_DEPENDENCY: (engagementId: number) => `/api/engagements/${engagementId}/workspace/dependencies`,
  UPDATE_DEPENDENCY: (engagementId: number, dependencyId: number) => `/api/engagements/${engagementId}/workspace/dependencies/${dependencyId}`,
};

export const DEPENDENCY_MESSAGES = {
  LOAD_ERROR: 'Failed to load dependencies.',
  CREATE_SUCCESS: 'Dependency created successfully.',
  CREATE_ERROR: 'Failed to create dependency.',
  UPDATE_SUCCESS: 'Dependency updated successfully.',
  UPDATE_ERROR: 'Failed to update dependency.',
};
