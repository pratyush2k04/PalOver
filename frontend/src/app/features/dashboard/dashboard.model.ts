export interface RecentActivityItem {
  id: string;
  description: string;
  timestamp: string;
  actor: string;
}

export interface RecentChecklistUpdate {
  id: string;
  itemName: string;
  category: string;
  updatedAt: string;
}

export interface DashboardSummary {
  latestPublishedVersion: {
    versionLabel: string;
  } | null;
  pendingDraftChangesCount: number;
  quickStatistics: {
    totalItems: number;
    totalCategories: number | null;
    activeVersion: string;
  };
  recentActivity: RecentActivityItem[];
  recentChecklistUpdates: RecentChecklistUpdate[];
}
