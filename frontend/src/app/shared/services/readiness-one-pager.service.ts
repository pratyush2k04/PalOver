import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface OutstandingMandatoryItemDto {
  category: string;
  sourceId: number | null;
  label: string;
  reason: string;
}

export interface OpenBlockerDto {
  riskId: number;
  title: string;
  status: string;
  impact: string;
  ownerName: string | null;
}

export interface CommitmentComparisonDto {
  commitmentId: number;
  title: string;
  committedTimeline: string;
  realisticTimeline: string | null;
  isAcknowledged: boolean;
  acknowledgementMessage: string | null;
}

export interface CommercialDetailDto {
  commercialModelName: string;
  commercialDocumentTitles: string[];
}

export interface ReadinessOnePagerDto {
  engagementId: number;
  engagementName: string;
  clientName: string;
  requesterId: number;
  requesterName: string;
  generatedAtUtc: string;
  score: number;
  totalApplicable: number;
  totalPassed: number;
  handoverFieldsApplicable: number;
  handoverFieldsPassed: number;
  commitmentsApplicable: number;
  commitmentsPassed: number;
  accessItemsApplicable: number;
  accessItemsPassed: number;
  kickoffDate: string;
  billingStartDate: string | null;
  daysToKickoff: number;
  daysToKickoffTier: string;
  requiredScoreThresholdForTier: number;
  status: string;
  outstandingMandatoryItems: OutstandingMandatoryItemDto[];
  openBlockers: OpenBlockerDto[];
  commitments: CommitmentComparisonDto[];
  hasResourcingGap: boolean;
  unconfirmedResourceCount: number;
  daysToBilling: number | null;
  isDayOneSignedOff: boolean;
  dayOneSignOffIsException: boolean;
  commercial: CommercialDetailDto | null;
  scoreExclusionNotes: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ReadinessOnePagerService {
  private http = inject(HttpClient);

  getOnePager(engagementId: number): Observable<ReadinessOnePagerDto> {
    return this.http.get<ReadinessOnePagerDto>(`/api/Engagements/${engagementId}/readiness-one-pager`);
  }
}
