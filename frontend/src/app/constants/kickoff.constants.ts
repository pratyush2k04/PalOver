export const KICKOFF_MESSAGES = {
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
