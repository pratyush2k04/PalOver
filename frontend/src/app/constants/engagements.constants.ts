export const ENGAGEMENT_API_ENDPOINTS = {
  MARK_SOW_SIGNED: '/api/Engagements/mark-sow-signed',
  ENGAGEMENT_TYPES: '/api/EngagementTypes',
  COMMERCIAL_MODELS: '/api/CommercialModels',
  ENGAGEMENT_INFO: (id: number) => `/api/Engagements/${id}`,
  ENGAGEMENT_OWNERS: (id: number) => `/api/Engagements/${id}/owners`,
  ASSIGN_PROJECT_MANAGER: (id: number) => `/api/Engagements/${id}/project-manager`,
  ASSIGN_TECH_LEAD: (id: number) => `/api/Engagements/${id}/tech-lead`,
  MY_ENGAGEMENTS: '/api/Engagements/my-engagements',
} as const;

export const ENGAGEMENT_MESSAGES = {
  LOAD_ENGAGEMENTS_ERROR: 'Failed to load your engagements. Please try again.',
  CREATE_ENGAGEMENT_ERROR: 'Failed to create engagement. Please try again.',
} as const;
