export enum NotificationPriority {
  Low = 1,
  Medium = 2,
  High = 3
}

export interface Notification {
  id: number;
  title: string;
  body: string;
  priority: NotificationPriority;
  status: string;
  scheduledFor: string;
  receivedAt?: string;
  readAt: string | null;
  link: string | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface NotificationFilterParams {
  engagementId?: number;
  page?: number;
  pageSize?: number;
  status?: 'all' | 'unread' | 'read';
  priority?: NotificationPriority | 'all';
}

export interface BulkDeleteNotificationsRequest {
  ids: number[];
}
