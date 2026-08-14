export interface LeadershipEngagementListItem {
  engagementId: number;
  engagementName: string;
  clientName: string;
  pmName: string | null;
  readinessScore: number;
  openBlockerCount: number;
  daysToKickoff: number;
  status: 'Kickoff-ready' | 'In progress' | 'At risk' | string;
}

export type StatusFilterType = 'ALL' | 'Kickoff-ready' | 'In progress' | 'At risk';
export type DaysFilterType = 'ALL' | '7' | '14' | '30';
