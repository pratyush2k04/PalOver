export const TECHNICAL_READINESS_ROUTES = {
  BASE: 'technical-readiness'
} as const;

export const TECHNICAL_READINESS_ROLES = {
  TECH_LEAD: 'Tech Lead'
} as const;

export const TECHNICAL_READINESS_ALERTS = {
  SHARE_SUMMARY: 'Day-1 Summary shared with team!'
} as const;

export const TECHNICAL_READINESS_API_ENDPOINTS = {
  ACCESS_ITEMS: (engagementId: number) => `/api/engagements/${engagementId}/access-items`,
  ACCESS_ITEM: (id: number) => `/api/access-items/${id}`,
  ACCESS_ITEM_STATUS: (id: number) => `/api/access-items/${id}/status`,
  ACCESS_ITEM_OWNER: (id: number) => `/api/access-items/${id}/owner`,
  ENVIRONMENT_TOOLING_ITEMS: (engagementId: number) => `/api/engagements/${engagementId}/environment-tooling`,
  ENVIRONMENT_TOOLING_STATUS: (id: number) => `/api/environment-tooling/${id}/status`,
  ENVIRONMENT_TOOLING_OWNER: (id: number) => `/api/environment-tooling/${id}/owner`,
  BROWNFIELD_FINDINGS: (engagementId: number) => `/api/engagements/${engagementId}/brownfield-findings`,
  BROWNFIELD_DECISION: (id: number) => `/api/brownfield-findings/${id}/decision`,
  BROWNFIELD_ITEM: (id: number) => `/api/brownfield-findings/${id}`,
  ARCHITECTURE_VALIDATION: (engagementId: number) => `/api/engagements/${engagementId}/architecture-validation`,
  ARCHITECTURE_VALIDATION_FEATURES: (engagementId: number) => `/api/engagements/${engagementId}/architecture-validation/features`,
  ARCHITECTURE_VALIDATION_FEATURE_ITEM: (featureId: number) => `/api/architecture-validation/features/${featureId}`
} as const;

export const TECHNICAL_READINESS_MESSAGES = {
  LOAD_ACCESS_ITEMS_ERROR: 'Failed to load access tracker items.',
  ADD_ACCESS_ITEM_ERROR: 'Failed to add custom access item.',
  UPDATE_ACCESS_STATUS_ERROR: 'Failed to update access item status.',
  UPDATE_ACCESS_OWNER_ERROR: 'Failed to update access item owner.',
  DELETE_ACCESS_ITEM_ERROR: 'Failed to delete access item.',
  LOAD_TOOLING_ITEMS_ERROR: 'Failed to load environment and tooling items.',
  ADD_TOOLING_ITEM_ERROR: 'Failed to add custom environment/tooling item.',
  UPDATE_TOOLING_STATUS_ERROR: 'Failed to update environment/tooling status.',
  UPDATE_TOOLING_OWNER_ERROR: 'Failed to update environment/tooling owner.'
} as const;
