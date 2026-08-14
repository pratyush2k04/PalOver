export const NOTIFICATIONS_API_ENDPOINTS = {
  NOTIFICATIONS: '/api/notifications',
  UNREAD_COUNT: '/api/notifications/unread-count',
  MARK_AS_READ: (id: number) => `/api/notifications/${id}/read`,
  MARK_ALL_AS_READ: '/api/notifications/read-all',
} as const;
