export enum StandardRoleEnum {
  SeniorDeveloper = 1,
  Developer = 2,
  QAEngineer = 3,
  SolutionArchitect = 4,
  BusinessAnalyst = 5,
}

export enum SkillEnum {
  ReactFrontend = 1,
  AngularFrontend = 2,
  NodeJsBackend = 3,
  DotNetBackend = 4,
  DataEngineering = 5,
  QAAutomation = 6,
  DevOpsCloud = 7,
  BusinessAnalysis = 8,
}

export enum ResourceRequestStatus {
  Raised = 'Raised',
  Allocated = 'Allocated',
  Confirmed = 'Confirmed',
}

export interface ResourceAllocationItem {
  id: number;
  name: string;
  allocatedDate: string;
  isConfirmed: boolean;
  confirmedDate: string | null;
}

export interface ResourceRequestItem {
  id: number;
  engagementId: number;
  standardRole: string;
  skills: string[];
  peopleNeeded: number;
  requiredByDate: string;
  jobDescription: string;
  status: string;
  requestedById: number;
  isUrgent: boolean;
  allocations: ResourceAllocationItem[];
}

export interface CreateResourceRequestPayload {
  standardRole: number;
  skills: number[];
  peopleNeeded: number;
  requiredByDate: string;
  jobDescription: string;
}

export interface AllocateResourcesPayload {
  names: string[];
}

export interface ConfirmAllocationPayload {
  confirmedDate: string;
}

export interface RemoveAllocationPayload {
  reason: string;
}

export interface ResourcingGapDto {
  hasGap: boolean;
  unconfirmedCount: number;
  daysToBilling: number | null;
}
