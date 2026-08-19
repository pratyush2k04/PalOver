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
  HANDOVER_QUERY_RESPOND: (engagementId: number, queryId: number) =>
    `/api/engagements/${engagementId}/handover/queries/${queryId}/respond`,
  HANDOVER_QUERY_TOGGLE_RESOLUTION: (engagementId: number, queryId: number) =>
    `/api/engagements/${engagementId}/handover/queries/${queryId}/toggle-resolution`,
} as const;

export const HANDOVER_MESSAGES = {
  LOAD_ERROR: 'Failed to load handover details. Please try again.',
  SAVE_DRAFT_SUCCESS: 'Handover draft saved successfully.',
  SUBMIT_SUCCESS: 'Handover submitted successfully.',
} as const;
