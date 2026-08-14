export const RISK_ROUTES = {
  BASE: 'risks'
} as const;

export const RISK_API_ENDPOINTS = {
  GET_RISKS: (engagementId: number) => `/api/engagements/${engagementId}/workspace/risks`,
  CREATE_RISK: (engagementId: number) => `/api/engagements/${engagementId}/workspace/risks`,
  UPDATE_RISK: (engagementId: number, riskId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}`,
  CLOSE_RISK: (engagementId: number, riskId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}/close`,
  REOPEN_RISK: (engagementId: number, riskId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}/reopen`
} as const;

export const RISK_MESSAGES = {
  LOAD_ERROR: 'Failed to load risks register.',
  CREATE_SUCCESS: 'Risk recorded successfully.',
  CREATE_ERROR: 'Failed to record risk.',
  UPDATE_SUCCESS: 'Risk updated successfully.',
  UPDATE_ERROR: 'Failed to update risk.',
  CLOSE_SUCCESS: 'Risk closed successfully.',
  CLOSE_ERROR: 'Failed to close risk. Only Project Managers or Risk Owners can close risks.',
  REOPEN_SUCCESS: 'Risk reopened successfully.',
  REOPEN_ERROR: 'Failed to reopen risk.',
  REOPEN_CONFIRM: (title: string) => `Are you sure you want to reopen the risk "${title}"?`
} as const;
