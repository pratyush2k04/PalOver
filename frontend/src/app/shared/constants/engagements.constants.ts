export const ENGAGEMENT_API_ENDPOINTS = {
  MARK_SOW_SIGNED: '/api/Engagements/mark-sow-signed',
  ENGAGEMENT_TYPES: '/api/EngagementTypes',
  COMMERCIAL_MODELS: '/api/CommercialModels',
  ENGAGEMENT_INFO: (id: number) => `/api/Engagements/${id}`,
  ENGAGEMENT_OWNERS: (id: number) => `/api/Engagements/${id}/owners`,
  ASSIGN_PROJECT_MANAGER: (id: number) => `/api/Engagements/${id}/project-manager`,
  ASSIGN_TECH_LEAD: (id: number) => `/api/Engagements/${id}/tech-lead`,
  MY_ENGAGEMENTS: '/api/Engagements/my-engagements',
  ALL_ENGAGEMENTS: '/api/Engagements',
  LEADERSHIP_USERS: (id: number) => `/api/Engagements/${id}/leadership-users`,
  LEADERSHIP_ROLE_USERS: '/api/Roles/4/users',
  DELETE_LEADERSHIP_USER: (id: number, userId: number) => `/api/Engagements/${id}/leadership-users/${userId}`,
  ENGAGEMENT_CHECKLIST: (id: number) => `/api/engagements/${id}/workspace/checklist`,
  ENGAGEMENT_DOCUMENTS: (id: number) => `/api/documents/engagement/${id}`,
  DOCUMENT_CATEGORIES: '/api/documents/categories',
  DOCUMENT_UPLOAD: '/api/documents/upload',
  DOCUMENT_DOWNLOAD: (id: number) => `/api/documents/${id}/download`,
  DOCUMENT_DELETE: (id: number) => `/api/documents/${id}`,
} as const;

export const ENGAGEMENT_CONSTANTS = {
  PROJECT_MANAGEMENT_ROLE_ID: 2,
  TECH_LEAD_ROLE_ID: 3,
  KICKOFF_PENDING_STATUS: 'KickoffPending',
} as const;

export const ENGAGEMENT_MESSAGES = {
  LOAD_ENGAGEMENTS_ERROR: 'Failed to load your engagements. Please try again.',
  CREATE_ENGAGEMENT_ERROR: 'Failed to create engagement. Please try again.',
  LOAD_CHECKLIST_ERROR: 'Failed to load the engagement checklist. Please try again.',
  LOAD_DOCUMENTS_ERROR: 'Failed to load documents. Please try again.',
  UPLOAD_DOCUMENT_ERROR: 'Failed to upload document. Please try again.',
  DELETE_DOCUMENT_ERROR: 'Failed to delete document. Please try again.',
  ASSIGNMENT_SUCCESS: (name: string, label: string) => `${name} assigned as ${label}.`,
  ASSIGNMENT_ERROR: 'Failed to save assignment. Please try again.',
} as const;

export const ENGAGEMENT_CHECKLIST_MESSAGES = {
  LOAD_FAILED: 'Failed to load the engagement checklist. Please try again.'
} as const;

export const ENGAGEMENT_WORKSPACE_CONFIG = {
  DEFAULT_TABS: [
    { label: 'Overview', route: 'overview', enabled: true },
    { label: 'Handover', route: 'handover', enabled: true },
    { label: 'Documents', route: 'documents', enabled: true },
    { label: 'People', route: 'people', enabled: true },
    { label: 'Checklist', route: 'checklist', enabled: true },
    { label: 'Kickoff', route: 'kickoff', enabled: true },
    { label: 'Resourcing', route: 'resourcing', enabled: false },
    { label: 'Technical Readiness', route: 'technical-readiness', enabled: true },
    { label: 'Risks', route: 'risks', enabled: true },
    { label: 'Reporting', route: 'reporting', enabled: false }
  ]
} as const;
