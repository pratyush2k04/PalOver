export const DOCUMENT_API_ENDPOINTS = {
  BY_ENGAGEMENT: (engagementId: number) => `/api/documents/engagement/${engagementId}`,
  CATEGORIES: '/api/documents/categories',
  UPLOAD: '/api/documents/upload',
  DOWNLOAD: (id: number) => `/api/documents/${id}/download`,
  REMOVE: (id: number) => `/api/documents/${id}/remove`,
  VISIBILITY: (id: number) => `/api/documents/${id}/visibility`
} as const;
