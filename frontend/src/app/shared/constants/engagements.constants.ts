import { ENGAGEMENT_ROUTES } from './routes.constants';

export const ENGAGEMENT_API_ENDPOINTS = {
  MARK_SOW_SIGNED: '/api/Engagements/mark-sow-signed',
  ENGAGEMENT_TYPES: '/api/EngagementTypes',
  COMMERCIAL_MODELS: '/api/CommercialModels',
  ENGAGEMENT_INFO: (id: number) => `/api/Engagements/${id}`,
  ENGAGEMENT_OWNERS: (id: number) => `/api/Engagements/${id}/owners`,
  ENGAGEMENT_USERS: (id: number) => `/api/Engagements/${id}/users`,
  ASSIGN_PROJECT_MANAGER: (id: number) => `/api/Engagements/${id}/project-manager`,
  ASSIGN_TECH_LEAD: (id: number) => `/api/Engagements/${id}/tech-lead`,
  MY_ENGAGEMENTS: '/api/Engagements/my-engagements',
  MY_ENGAGEMENTS_PAGE: '/api/Engagements/my-engagements/page',
  LEADERSHIP_DASHBOARD: '/api/engagements/leadership-dashboard',
  ALL_ENGAGEMENTS: '/api/Engagements',
  LEADERSHIP_USERS: (id: number) => `/api/Engagements/${id}/leadership-users`,
  UPDATE_LEADERSHIP_USERS: (id: number) => `/api/Engagements/${id}/leadership-users`,
  LEADERSHIP_ROLE_USERS: '/api/Roles/4/users',
  DELETE_LEADERSHIP_USER: (id: number, userId: number) => `/api/Engagements/${id}/leadership-users/${userId}`,
  HANDOVER_DRAFT_STATUS: (id: number) => `/api/Engagements/${id}/handover-draft-status`,
  ENGAGEMENT_CHECKLIST: (id: number) => `/api/engagements/${id}/workspace/checklist`,
  CHECKLIST_ITEM_EVIDENCE: (engagementId: number, itemId: number) => `/api/engagements/${engagementId}/workspace/checklist/items/${itemId}/evidence`,
  ENGAGEMENT_DOCUMENTS: (id: number) => `/api/documents/engagement/${id}`,
  DOCUMENT_CATEGORIES: '/api/documents/categories',
  DOCUMENT_UPLOAD: '/api/documents/upload',
  DOCUMENT_DOWNLOAD: (id: number) => `/api/documents/${id}/download`,
  DOCUMENT_DELETE: (id: number) => `/api/documents/${id}`,
  OPEN_ITEMS_SUMMARY: (id: number) => `/api/Engagements/${id}/open-items-summary`,
  DAY_ONE_SIGN_OFF: (id: number) => `/api/Engagements/${id}/day-one-sign-off`,
  EVIDENCE_SNAPSHOT: (id: number) => `/api/Engagements/${id}/evidence-snapshot`,
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

export const OPEN_ITEMS_ERROR_MESSAGES = {
  ENGAGEMENT_NOT_FOUND: 'Engagement not found',
  LOAD_FAILED: 'Failed to load open items'
} as const;

export const ENGAGEMENT_CHECKLIST_MESSAGES = {
  LOAD_FAILED: 'Failed to load the engagement checklist. Please try again.',
  EVIDENCE_SAVED_SUCCESS: 'Evidence saved successfully.'
} as const;

export const ENGAGEMENT_WORKSPACE_CONFIG = {
  DEFAULT_TABS: [
    { label: 'Overview', route: ENGAGEMENT_ROUTES.OVERVIEW, enabled: true },
    { label: 'Handover', route: ENGAGEMENT_ROUTES.HANDOVER, enabled: true },
    { label: 'Documents', route: ENGAGEMENT_ROUTES.DOCUMENTS, enabled: true },
    { label: 'People', route: ENGAGEMENT_ROUTES.PEOPLE, enabled: true },
    { label: 'Checklist', route: ENGAGEMENT_ROUTES.CHECKLIST, enabled: true },
    { label: 'Kickoff', route: ENGAGEMENT_ROUTES.KICKOFF, enabled: true },
    { label: 'Resources', route: ENGAGEMENT_ROUTES.RESOURCES, enabled: true },
    { label: 'Technical Readiness', route: ENGAGEMENT_ROUTES.TECHNICAL_READINESS, enabled: true },
    { label: 'Risks', route: ENGAGEMENT_ROUTES.RISKS, enabled: true },
    { label: 'Assumptions & Dependencies', route: ENGAGEMENT_ROUTES.ASSUMPTIONS, enabled: true },
    { label: 'Audits', route: ENGAGEMENT_ROUTES.AUDITS, enabled: true },
    { label: 'Sign Off', route: ENGAGEMENT_ROUTES.SIGN_OFF, enabled: false }
  ]
} as const;
