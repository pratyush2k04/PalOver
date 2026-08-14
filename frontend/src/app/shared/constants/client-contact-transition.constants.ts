export const CLIENT_CONTACT_TRANSITION_ENDPOINTS = {
  BASE: (engagementId: number) => `/api/engagements/${engagementId}/client-contact-transition`,
  HISTORY: (engagementId: number) => `/api/engagements/${engagementId}/client-contact-transition/history`,
} as const;

export const FORM_LABELS = {
  CARD_TITLE: 'Client Contact Transition',
  ADD_TRANSITION_TITLE: 'Add Client Contact Transition',
  OUTGOING_SECTION: 'Outgoing Contact',
  OUTGOING_NAME: 'Name (Required)',
  OUTGOING_ROLE: 'Role (Required)',
  INCOMING_SECTION: 'Incoming Contact',
  INCOMING_NAME: 'Name (Required)',
  INCOMING_ROLE: 'Role (Required)',
} as const;

export const FORM_PLACEHOLDERS = {
  OUTGOING_NAME: 'Current contact name',
  OUTGOING_ROLE: 'e.g., Sales Manager, Account Lead',
  INCOMING_NAME: 'New contact name',
  INCOMING_ROLE: 'e.g., Delivery Manager, PM',
} as const;

export const BUTTON_LABELS = {
  ADD_TRANSITION: '+ Add Transition',
  REVIEW_CONFIRM: 'Review & Confirm',
  BACK: 'Back',
  CANCEL: 'Cancel',
  CONFIRM_SAVE: 'Confirm & Save',
  SAVING: 'Saving...',
} as const;

export const DIALOG_LABELS = {
  CONFIRM_TITLE: 'Confirm Client Contact Transition',
  CONFIRM_MESSAGE: 'Please review the details below. This transition cannot be edited after saving.',
  OUTGOING: 'Outgoing:',
  INCOMING: 'Incoming:',
  PRESENT: 'Present:',
  NO_ROLE: 'no role',
} as const;

export const VALIDATION_MESSAGES = {
  REQUIRED: 'This field is required.',
  MAX_LENGTH: 'Maximum 200 characters.',
  OUTGOING_NAME_REQUIRED: 'Outgoing contact name is required.',
  OUTGOING_ROLE_REQUIRED: 'Outgoing contact role is required.',
  INCOMING_NAME_REQUIRED: 'Incoming contact name is required.',
  INCOMING_ROLE_REQUIRED: 'Incoming contact role is required.',
} as const;

export const STATUS_MESSAGES = {
  LOADING: 'Loading transition history...',
  SAVE_SUCCESS: 'Client contact transition saved successfully.',
  SAVE_ERROR: 'Failed to save transition. Please try again.',
  LOAD_ERROR: 'Failed to load transition history.',
} as const;
