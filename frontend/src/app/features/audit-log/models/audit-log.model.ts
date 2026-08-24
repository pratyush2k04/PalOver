export type AuditAction = 
  | 'UserLoggedIn' 
  | 'UserCreated' 
  | 'UserUpdated' 
  | 'UserDeleted' 
  | 'DocumentVersioned' 
  | 'DocumentUploaded' 
  | 'UnauthorizedAccess' 
  | 'AccountLockout' 
  | 'ClientContactDetailsChanged' 
  | 'ClientPersonaChanged' 
  | 'LeadershipAllocated' 
  | 'UserRoleAllocated' 
  | 'PmAndTlAssigned' 
  | 'PmAndTlReassigned' 
  | 'DayOneSummaryGenerated' 
  | 'MasterChecklistPublished' 
  | 'BrownfieldItemAdded' 
  | 'DiscoveryItemAdded'
  | 'ResourcingGapFlagRaised'
  | 'ResourcingGapFlagCleared'
  | 'ChecklistItemEvidenceUpdated'
  | 'DependencyCreated'
  | 'DependencyUpdated'
  | 'DependencyDeleted'
  | 'DependencyEscalated'
  | 'ReadinessOnePagerGenerated';
export interface AuditLogDto {
  id: number;
  userId: number;
  userName: string;
  action: AuditAction;
  details: string;
  createdAt: string;
  engagementId?: number | null;
}

export interface EngagementAuditLogDto {
  id: number;
  userId: number;
  userName: string;
  action: AuditAction;
  details: string;
  createdAt: string;
}

export type AuditCategory = 'Document' | 'Checklist' | 'Risk' | 'Access' | 'Handover' | 'Escalation' | 'ResourceAllocation' | 'TechnicalReadiness';

export interface SimpleUserDto {
  id: number;
  name: string;
}

export interface AuditLogFilters {
  search: string;
  fromDate: string | null;
  toDate: string | null;
  userId: number | null;
  categories: AuditCategory[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
