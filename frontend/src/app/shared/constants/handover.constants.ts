export const HANDOVER_API_ENDPOINTS = {
  HANDOVER: (id: number) => `/api/engagements/${id}/handover`,
  HANDOVER_DRAFT: (id: number) => `/api/engagements/${id}/handover/draft`,
  HANDOVER_SUBMIT: (id: number) => `/api/engagements/${id}/handover/submit`,
  HANDOVER_COMMITMENTS: (id: number) => `/api/engagements/${id}/handover/commitments`,
  HANDOVER_COMMITMENT_BY_ID: (engagementId: number, commitmentId: number) =>
    `/api/engagements/${engagementId}/handover/commitments/${commitmentId}`,
  HANDOVER_REALISTIC_COMMITMENT: (engagementId: number, commitmentId: number) =>
    `/api/engagements/${engagementId}/handover/commitments/${commitmentId}/realistic`,
} as const;

export const MAX_FREE_TEXT_LENGTH = 2000;
export const HANDOVER_SENTIMENT_OPTIONS = ['New', 'Warm', 'Strained'] as const;
export const HANDOVER_TIMELINE_UNITS = ['Days', 'Weeks', 'Months'] as const;

export const HANDOVER_LABELS = {
  TITLE: 'Pre-Sales to Delivery Handover',
  CLIENT_CONTEXT: 'Client Context',
  COMMITMENTS: 'Commitments',
  CLIENT_BACKGROUND: 'Client Background',
  BUSINESS_PROBLEM: 'Business Problem *',
  CLIENT_SENTIMENT: 'Client Sentiment *',
  DEAL_ORIGIN: 'Deal Origin',
  PROMISED: 'Promised',
  REALISTIC: 'Realistic',
  SOFTWARE: 'Software',
  SERVICES: 'Services',
  TRAINING: 'Training',
  ESCALATED_LEADERSHIP: 'Escalated to Leadership',
  CLICK_TO_EDIT: 'Click here to give realistic estimates',
  NOT_YET_PROVIDED: 'Not yet provided.',
  NOT_PROVIDED: 'Not provided',
  REMOVE_CONFIRM: 'Remove this commitment?',
  ACK_WARNING: "You've changed your realistic estimates. Consider revisiting your acknowledgement message.",
  ADD_NEW_COMMITMENT: 'Add New Commitment',
  ACKNOWLEDGEMENT_MESSAGE_REQ: 'Acknowledgement Message *',
  ACKNOWLEDGEMENT_MESSAGE: 'Acknowledgement Message',
  COMMITMENT_TYPE: 'Commitment Type',
  DESCRIPTION: 'Description',
  TIMELINE: 'Timeline',
  UNIT: 'Unit',
} as const;

export const HANDOVER_PLACEHOLDERS = {
  CLIENT_BACKGROUND: 'Brief history and overview of the client...',
  BUSINESS_PROBLEM: 'What core problem are we solving?',
  CLIENT_SENTIMENT: '-Select-',
  DEAL_ORIGIN: 'e.g. RFP, Direct Sales, Partner...',
  TITLE_REQ: 'Enter commitment title...',
  DESCRIPTION_REQ: 'Enter commitment description...',
  TITLE: 'Title',
  DESCRIPTION: 'Description',
  REALISTIC_DESCRIPTION: 'Realistic description',
  WHY_REALISTIC_DIFFERS: 'Why does the realistic estimate differ?',
  TIMELINE: 'Timeline',
} as const;

export const HANDOVER_BUTTONS = {
  SAVE_DRAFT: 'Save Draft',
  SUBMIT_HANDOVER: 'Submit Handover',
  SAVE: 'Save',
  CANCEL: 'Cancel',
  DELETE: 'Delete',
  YES: 'Yes',
  ADD_COMMITMENT: 'Add Commitment',
  ACKNOWLEDGE_MISMATCH: 'Acknowledge Timeline Mismatch',
} as const;

export const HANDOVER_MESSAGES = {
  LOAD_ERROR: 'Failed to load handover details',
  SAVE_DRAFT_SUCCESS: 'Draft saved successfully',
  SAVE_DRAFT_ERROR: 'Failed to save draft',
  SUBMIT_SUCCESS: 'Handover submitted successfully',
  SUBMIT_ERROR: 'Failed to submit handover',
  DRAFT_REQUIRED_FIELDS: 'Business Problem and Client Sentiment are required to save a draft.',
  SUBMIT_REQUIRED_FIELDS: 'Please complete all required fields.',
  KICKOFF_NOT_READY: 'Engagement is not kickoff-ready. All mandatory items must be complete before submitting the handover.',
  COMMITMENT_ADDED: 'Commitment added successfully',
  COMMITMENT_ADD_ERROR: 'Failed to add commitment',
  COMMITMENT_UPDATED: 'Commitment updated successfully',
  COMMITMENT_UPDATE_ERROR: 'Failed to update commitment',
  COMMITMENT_DELETED: 'Commitment deleted',
  COMMITMENT_DELETE_ERROR: 'Failed to delete commitment',
  REALISTIC_SAVED: 'Realistic commitment saved',
  REALISTIC_SAVE_ERROR: 'Failed to save realistic commitment',
  CLIENT_CONTACT_TRANSITION_SAVED: 'Client contact transition recorded.',
  TITLE_REQUIRED: 'Title is required.',
  DESCRIPTION_REQUIRED: 'Description is required.',
  TITLE_AND_DESC_REQUIRED: 'Title and Description are required.',
  ACK_MESSAGE_REQUIRED: 'An acknowledgement message is required when the realistic timeline differs from the promised timeline.',
  SAVE_DRAFT_FIRST: 'Save draft first to add commitments.',
} as const;
