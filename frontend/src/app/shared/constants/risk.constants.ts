export const RISK_API_ENDPOINTS = {
  GET_RISKS: (engagementId: number) => `/api/engagements/${engagementId}/workspace/risks`,
  CREATE_RISK: (engagementId: number) => `/api/engagements/${engagementId}/workspace/risks`,
  UPDATE_RISK: (engagementId: number, riskId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}`,
  CLOSE_RISK: (engagementId: number, riskId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}/close`,
  REOPEN_RISK: (engagementId: number, riskId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}/reopen`
} as const;

export const RISK_COMMENT_API_ENDPOINTS = {
  GET_COMMENTS: (engagementId: number, riskId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}/comments`,
  ADD_COMMENT: (engagementId: number, riskId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}/comments`,
  UPDATE_COMMENT: (engagementId: number, riskId: number, commentId: number) => `/api/engagements/${engagementId}/workspace/risks/${riskId}/comments/${commentId}`,
  GET_MEMBERS: (engagementId: number) => `/api/engagements/${engagementId}/workspace/risks/members`
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

export const RISK_COMMENT_MESSAGES = {
  LOAD_ERROR: 'Failed to load risk comments.',
  MEMBERS_LOAD_ERROR: 'Failed to load engagement members.',
  ADD_SUCCESS: 'Comment posted successfully.',
  ADD_ERROR: 'Failed to post comment.',
  UPDATE_SUCCESS: 'Comment updated.',
  UPDATE_ERROR: 'Failed to update comment.',
  EDIT_WINDOW_EXPIRED: 'Comments can only be edited within 15 minutes of creation.'
} as const;

export const RISK_COMMENT_CONSTANTS = {
  MAX_LENGTH: 1000,
  TEXT_EXCEED_ERROR: 'Comment content cannot exceed 1000 characters.',
  EMPTY_DISCUSSION_MSG: 'No comments yet. Start the discussion below.',
  INPUT_PLACEHOLDER: "Add a comment, @mention a teammate..."
} as const;

