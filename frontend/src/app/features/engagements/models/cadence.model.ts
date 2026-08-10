export enum CadenceTypeEnum {
  Daily = 1,
  Weekly = 2,
  Fortnightly = 3,
  Monthly = 4,
  Quarterly = 5
}

export interface CommunicationCadence {
  id: number;
  stakeholderRole: string;
  cadence: string;
  participants?: string;
}

export interface CreateCommunicationCadence {
  stakeholderRole: string;
  cadence: CadenceTypeEnum;
  participantUserIds: number[];
}

export interface EngagementParticipant {
  userId: number;
  fullName: string;
}
