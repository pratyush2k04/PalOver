export interface LeadershipUserItem {
  id: number;
  fullName: string;
  email: string;
}

export interface EngagementAllocationItem {
  engagementId: number;
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementType: string;
  commercialModel: string;
  kickoffDate: string;
  billingStartDate?: string;
  status: string;
  isKickoffReady: boolean;
  leadershipUsers?: LeadershipUserItem[];
  isLoadingLeadership?: boolean;
}
