export const ENGAGEMENT_API_ENDPOINTS = {
  MARK_SOW_SIGNED: '/api/Engagements/mark-sow-signed',
  ENGAGEMENT_TYPES: '/api/EngagementTypes',
  COMMERCIAL_MODELS: '/api/CommercialModels',
  ENGAGEMENT_INFO: (id: number) => `/api/Engagements/${id}`,
  ENGAGEMENT_OWNERS: (id: number) => `/api/Engagements/${id}/owners`,
  ASSIGN_PROJECT_MANAGER: (id: number) => `/api/Engagements/${id}/project-manager`,
  ASSIGN_TECH_LEAD: (id: number) => `/api/Engagements/${id}/tech-lead`,
  MY_ENGAGEMENTS: '/api/Engagements/my-engagements',
  ENGAGEMENT_CHECKLIST: (id: number) => `/api/engagements/${id}/workspace/checklist`,
  ENGAGEMENT_DOCUMENTS: (id: number) => `/api/documents/engagement/${id}`,
  DOCUMENT_CATEGORIES: '/api/documents/categories',
  DOCUMENT_UPLOAD: '/api/documents/upload',
  DOCUMENT_DOWNLOAD: (id: number) => `/api/documents/${id}/download`,
  DOCUMENT_DELETE: (id: number) => `/api/documents/${id}`,
} as const;

export const ENGAGEMENT_MESSAGES = {
  LOAD_ENGAGEMENTS_ERROR: 'Failed to load your engagements. Please try again.',
  CREATE_ENGAGEMENT_ERROR: 'Failed to create engagement. Please try again.',
  LOAD_CHECKLIST_ERROR: 'Failed to load the engagement checklist. Please try again.',
  LOAD_DOCUMENTS_ERROR: 'Failed to load documents. Please try again.',
  UPLOAD_DOCUMENT_ERROR: 'Failed to upload document. Please try again.',
  DELETE_DOCUMENT_ERROR: 'Failed to delete document. Please try again.',
} as const;
