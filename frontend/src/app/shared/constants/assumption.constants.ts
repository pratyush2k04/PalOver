export const ASSUMPTION_API_ENDPOINTS = {
  GET_ASSUMPTIONS: (engagementId: number) => `/api/engagements/${engagementId}/workspace/assumptions`,
  CREATE_ASSUMPTION: (engagementId: number) => `/api/engagements/${engagementId}/workspace/assumptions`,
  UPDATE_ASSUMPTION: (engagementId: number, id: number) => `/api/engagements/${engagementId}/workspace/assumptions/${id}`,
  VALIDATE_ASSUMPTION: (engagementId: number, id: number) => `/api/engagements/${engagementId}/workspace/assumptions/${id}/validate`
} as const;

export const ASSUMPTION_MESSAGES = {
  LOAD_ERROR: 'Failed to load assumptions.',
  CREATE_SUCCESS: 'Assumption created successfully.',
  CREATE_ERROR: 'Failed to create assumption.',
  UPDATE_SUCCESS: 'Assumption updated successfully.',
  UPDATE_ERROR: 'Failed to update assumption.',
  UPDATE_STATUS_SUCCESS: (status: string) => `Assumption marked as ${status}.`,
  UPDATE_STATUS_ERROR: 'Failed to update assumption status.'
} as const;
