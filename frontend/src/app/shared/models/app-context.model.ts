export interface HomeUser {
  id: number;
  fullName: string;
  role: string;
}

export interface DashboardCard {
  title: string;
  count: number;
  type: string;
}

export interface WorkspaceMetadata {
  route: string;
  title: string;
  welcomeMessage: string;
  emptyState: string;
  cards: DashboardCard[];
  criticalBlockers: DashboardBlocker[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface DashboardEngagement {
  engagementId: number;
  engagementName: string;
  kickoffDate: string;
  daysToGo: number;
  readinessPercentage: number;
  status: string;
}

export interface DashboardBlocker {
  riskId: number;
  projectName: string;
  dueDate: string;
}

export interface HomeResponse {
  user: HomeUser;
  home: WorkspaceMetadata;
}

export interface AppContext {
  user: HomeUser;
  workspace: WorkspaceMetadata;
}
