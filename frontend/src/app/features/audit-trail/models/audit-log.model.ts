export interface AuditLogDto {
  auditLogId: number;
  userId: number;
  userName: string;
  action: string;
  details: string;
  createdAt: string;
}

export interface CursorPagedResult<T> {
  items: T[];
  nextCursor: number | null;
  hasMore: boolean;
}
