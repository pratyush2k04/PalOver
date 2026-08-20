export const KICKOFF_MESSAGES = {
  READINESS_LOAD_ERROR: 'Failed to load kickoff readiness',
  MARKED_READY_SUCCESS: 'Engagement marked kickoff-ready',
  MARK_READY_ERROR: 'Failed to mark kickoff-ready',
  PLEASE_FIX_HIGHLIGHTED_FIELDS: 'Please fix the highlighted fields',
  ACTION_TITLE_REQUIRED: 'Action Title is required',
  OWNER_NAME_REQUIRED: 'Owner Name is required',
  DUE_DATE_REQUIRED: 'Due Date is required',
  OUTCOMES_SAVED_SUCCESS: 'Meeting outcomes saved',
  ACTION_CREATED_SUCCESS: 'Action item created successfully',
  ACTION_DELETED_SUCCESS: 'Action item deleted',
  STATUS_UPDATED_SUCCESS: (status: string) => `Status updated to ${status}`,
  DELETE_ACTION_CONFIRM: (title: string) => `Are you sure you want to delete "${title}"?`,
} as const;

export const KICKOFF_API_ENDPOINTS = {
  RECORD: (id: number) => `/api/engagements/${id}/workspace/kickoff`,
  ACTIONS_OPEN: (id: number) => `/api/engagements/${id}/workspace/kickoff/actions/open`,
  ACTIONS_CREATE: (id: number) => `/api/engagements/${id}/workspace/kickoff/actions`,
  ACTIONS_UPDATE_STATUS: (id: number, actionId: number) => `/api/engagements/${id}/workspace/kickoff/actions/${actionId}/status`,
  ACTIONS_UPDATE: (id: number, actionId: number) => `/api/engagements/${id}/workspace/kickoff/actions/${actionId}`,
  ACTIONS_DELETE: (id: number, actionId: number) => `/api/engagements/${id}/workspace/kickoff/actions/${actionId}`,
} as const;
