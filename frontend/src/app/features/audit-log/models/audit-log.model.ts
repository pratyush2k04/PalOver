export type AuditAction = 'UserLoggedIn' | 'UserCreated' | 'UserUpdated' | 'UserDeleted';

export interface AuditLogDto {
  id: number;
  userId: number;
  userName: string;
  action: AuditAction;
  details: string;
  createdAt: string;
}

export interface AuditLogFilters {
  search: string;
  fromDate: string | null;
  toDate: string | null;
}

export interface CursorPagedResult<T> {
  items: T[];
  nextCursor: number | null;
  hasMore: boolean;
}
