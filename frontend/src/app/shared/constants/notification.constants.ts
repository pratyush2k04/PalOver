export const NOTIFICATIONS_API_ENDPOINTS = {
  NOTIFICATIONS: '/api/notifications',
  UNREAD_COUNT: '/api/notifications/unread-count',
  MARK_AS_READ: (id: number) => `/api/notifications/${id}/read`,
  MARK_ALL_AS_READ: '/api/notifications/read-all',
  BULK_DELETE: '/api/notifications',
} as const;

export const NOTIFICATION_MESSAGES = {
  LOAD_ERROR: 'Failed to load notifications. Please try again.',
  MARK_READ_SUCCESS: 'Notification marked as read.',
  MARK_READ_ERROR: 'Failed to mark notification as read.',
  MARK_ALL_READ_SUCCESS: 'All notifications marked as read.',
  MARK_ALL_READ_ERROR: 'Failed to mark notifications as read.',
  DELETE_SUCCESS: 'Notification(s) deleted successfully.',
  DELETE_ERROR: 'Failed to delete notification(s).',
  REDIRECT_TOOLTIP: 'Click here to redirect'
} as const;
