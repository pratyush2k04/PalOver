export interface HomeUser {
  id: number;
  fullName: string;
  role: string;
  isAdmin: boolean;
}

export interface DashboardCard {
  title: string;
  count: number;
  subtitle?: string;
  type: string;
}

export interface WorkspaceMetadata {
  route: string;
  title: string;
  welcomeMessage: string;
  emptyState: string;
  cards: DashboardCard[];
  engagements: DashboardEngagement[];
  criticalBlockers: DashboardBlocker[];
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
  blockerDescription: string;
  ownerName: string;
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
