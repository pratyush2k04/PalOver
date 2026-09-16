export enum PersonType {
  Internal = 1,
  Client = 2
}

export interface PersonDto {
  personId: number;
  engagementId: number;
  name: string;
  role: string;
  personType: PersonType | string;
  userId?: number | null;
  clientBackground?: string | null;
  clientSentiment?: string | null;
  email?: string | null;
}
