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
  upcomingEngagements: DashboardEngagement[];
  totalUpcomingEngagements: number;
  criticalBlockers: DashboardBlocker[];
  totalCriticalBlockers: number;
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
  link: string | null;
}

export interface RawDashboardCard {
  type: string;
  count: number;
}

export interface RawHomeDashboard {
  cards: RawDashboardCard[];
  upcomingEngagements: DashboardEngagement[];
  totalUpcomingEngagements: number;
  criticalBlockers: DashboardBlocker[];
  totalCriticalBlockers: number;
}

export interface HomeResponse {
  user: HomeUser;
  home: RawHomeDashboard;
}

export interface AppContext {
  user: HomeUser;
  workspace: WorkspaceMetadata;
  rawDashboard?: RawHomeDashboard;
}
