export const HANDOVER_API_ENDPOINTS = {
  HANDOVER: (id: number) => `/api/engagements/${id}/handover`,
  HANDOVER_DRAFT: (id: number) => `/api/engagements/${id}/handover/draft`,
  HANDOVER_SUBMIT: (id: number) => `/api/engagements/${id}/handover/submit`,
  HANDOVER_COMMITMENTS: (id: number) => `/api/engagements/${id}/handover/commitments`,
  HANDOVER_COMMITMENT_BY_ID: (engagementId: number, commitmentId: number) =>
    `/api/engagements/${engagementId}/handover/commitments/${commitmentId}`,
  HANDOVER_REALISTIC_COMMITMENT: (engagementId: number, commitmentId: number) =>
    `/api/engagements/${engagementId}/handover/commitments/${commitmentId}/realistic`,
  HANDOVER_REVIEW: (id: number) => `/api/engagements/${id}/handover/review`,
  HANDOVER_QUERIES: (id: number) => `/api/engagements/${id}/handover/queries`,
  HANDOVER_QUERY_MESSAGES: (engagementId: number, queryId: number) =>
    `/api/engagements/${engagementId}/handover/queries/${queryId}/messages`,
  HANDOVER_QUERY_TOGGLE_RESOLUTION: (engagementId: number, queryId: number) =>
    `/api/engagements/${engagementId}/handover/queries/${queryId}/toggle-resolution`,
  HANDOVER_QUERY_BY_ID: (engagementId: number, queryId: number) =>
    `/api/engagements/${engagementId}/handover/queries/${queryId}`,
} as const;

export const MAX_FREE_TEXT_LENGTH = 2000;
export const HANDOVER_SENTIMENT_OPTIONS = ['New', 'Warm', 'Strained'] as const;
export const HANDOVER_TIMELINE_UNITS = ['Days', 'Weeks', 'Months'] as const;

export const HANDOVER_LABELS = {
  TITLE: 'Pre-Sales to Delivery Handover',
  CLIENT_CONTEXT: 'Client Context',
  COMMITMENTS: 'Commitments',
  CLIENT_BACKGROUND: 'Client Background',
  BUSINESS_PROBLEM: 'Business Problem',
  CLIENT_SENTIMENT: 'Client Sentiment',
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
  ADD_NEW_COMMITMENT: 'Add New Commitment',
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
  SAVE_DRAFT_FIRST: 'Save draft first to add commitments.',
  QUERY_LOAD_ERROR: 'Failed to load handover queries.',
  QUERY_MESSAGE_SUBMIT_SUCCESS: 'Message submitted.',
  QUERY_MESSAGE_SUBMIT_ERROR: 'Failed to submit message.',
  QUERY_UPDATE_ERROR: 'Failed to update query.',
  QUERY_RAISE_SUCCESS: 'Query raised.',
  QUERY_RAISE_ERROR: 'Failed to raise query.',
  QUERY_DELETED: 'Query deleted successfully.',
  QUERY_DELETE_ERROR: 'Failed to delete query.',
} as const;

export const HANDOVER_QUERY_FIELD_LABELS = {
  ClientBackground: 'Client Background',
  BusinessProblem: 'Business Problem',
  ClientSentiment: 'Client Sentiment',
  DealOrigin: 'Deal Origin',
  Commitment: 'Commitment',
} as const;

export const HANDOVER_READINESS_LABELS = {
  TITLE: 'Readiness Checklist',
  LOADING: 'Loading readiness checklist...',
  COMMERCIAL: 'COMMERCIAL',
  HANDOVER: 'HANDOVER',
  ITEMS: 'Items',
  ADD_ITEM: '+ Add Item',
  ITEM: 'Item',
  OWNER: 'Owner',
  EVIDENCE: 'Evidence',
  STATUS: 'Status',
  ACTIONS: 'Actions',
  BLOCK_REASON: 'Block Reason:',
  DOWNLOAD_EVIDENCE: 'Download evidence',
  VIEW_OR_REPLACE_EVIDENCE: 'View or replace evidence',
  ATTACH_EVIDENCE: '+ Attach',
  DELETE_ITEM: 'Delete Item',
  DELETE_CONFIRM_TITLE: 'Delete Handover Item',
  DELETE_CONFIRM_LABEL: 'Delete',
} as const;

export const HANDOVER_READINESS_MESSAGES = {
  LOAD_ERROR: 'Failed to load Handover Readiness items',
  STATUS_UPDATED: 'Status updated',
  STATUS_UPDATE_ERROR: 'Failed to update status',
  ADD_SUCCESS: (area: string) => `Custom ${area.toLowerCase()} item added successfully`,
  ADD_ERROR: 'Failed to add custom item',
  EVIDENCE_SAVE_SUCCESS: 'Evidence saved successfully',
  EVIDENCE_SAVE_ERROR: 'Failed to save evidence',
  DELETE_SUCCESS: 'Item deleted successfully',
  DELETE_ERROR: 'Failed to delete item',
} as const;
