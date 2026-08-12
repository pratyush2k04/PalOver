export const HANDOVER_API_ENDPOINTS = {
  HANDOVER: (id: number) => `/api/engagements/${id}/handover`,
  HANDOVER_DRAFT: (id: number) => `/api/engagements/${id}/handover/draft`,
  HANDOVER_SUBMIT: (id: number) => `/api/engagements/${id}/handover/submit`,
  HANDOVER_COMMITMENTS: (id: number) => `/api/engagements/${id}/handover/commitments`,
  HANDOVER_COMMITMENT_BY_ID: (engagementId: number, commitmentId: number) =>
    `/api/engagements/${engagementId}/handover/commitments/${commitmentId}`,
  HANDOVER_REALISTIC_COMMITMENT: (engagementId: number, commitmentId: number) =>
    `/api/engagements/${engagementId}/handover/commitments/${commitmentId}/realistic`,
  CLIENT_CONTACT_TRANSITION: (id: number) => `/api/engagements/${id}/client-contact-transition`,
  CLIENT_CONTACT_TRANSITION_CURRENT: (id: number) => `/api/engagements/${id}/client-contact-transition/current-contact`,
} as const;

export const HANDOVER_MESSAGES = {
  LOAD_ERROR: 'Failed to load handover details. Please try again.',
  SAVE_DRAFT_SUCCESS: 'Handover draft saved successfully.',
  SUBMIT_SUCCESS: 'Handover submitted successfully.',
} as const;

export const CLIENT_CONTACT_TRANSITION_MESSAGES = {
  CONFIRM_TITLE: 'Confirm Client Contact Transition',
  CONFIRM_LABEL: 'Confirm & Save',
  CANCEL_LABEL: 'Cancel',
  SAVE_SUCCESS: 'Client contact transition saved successfully.',
  SAVE_ERROR: 'Failed to save client contact transition.',
  EMPTY_STATE_PRESALES: 'No client contact transition recorded yet.',
  EMPTY_STATE_OTHER: 'No client contact transition recorded. Pre-Sales contact will be used.',
  VALIDATION_REQUIRED: 'This field is required.',
  VALIDATION_MAXLENGTH: 'Must not exceed 200 characters.',
  CONFIRM_NOTE: 'This cannot be edited afterward.',
} as const;
