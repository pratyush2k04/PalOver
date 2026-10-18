import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface KickoffReadinessDto {
  score: number;
  totalApplicable: number;
  totalPassed: number;
  handoverFieldsApplicable: number;
  handoverFieldsPassed: number;
  commitmentsApplicable: number;
  commitmentsPassed: number;
  checklistItemsApplicable: number;
  checklistItemsPassed: number;
}

@Injectable({
  providedIn: 'root'
})
export class KickoffReadinessService {
  private http = inject(HttpClient);

  private readinessSignal = signal<KickoffReadinessDto | null>(null);
  private isLoadingSignal = signal(true);
  private hasErrorSignal = signal(false);
  private trackedEngagementId: number | null = null;
  private requestSeq = 0;

  readonly readiness = this.readinessSignal.asReadonly();
  readonly isLoading = this.isLoadingSignal.asReadonly();
  readonly hasError = this.hasErrorSignal.asReadonly();

  /**
   * Ensures the shared store holds (or is loading) readiness for this engagement.
   * No-op if already tracking this engagement — safe to call from every consumer's
   * mount effect without triggering duplicate fetches.
   */
  track(engagementId: number): void {
    if (this.trackedEngagementId === engagementId) return;
    this.trackedEngagementId = engagementId;
    this.readinessSignal.set(null);
    this.fetch(engagementId);
  }

  /** Refetches readiness for the currently tracked engagement. Call after any mutation that could affect the score. */
  refresh(): void {
    if (this.trackedEngagementId !== null) {
      this.fetch(this.trackedEngagementId);
    }
  }

  private fetch(engagementId: number): void {
    const requestId = ++this.requestSeq;
    this.isLoadingSignal.set(true);
    this.hasErrorSignal.set(false);

    this.getReadiness(engagementId).subscribe({
      next: (data) => {
        if (requestId !== this.requestSeq) return;
        this.readinessSignal.set(data);
        this.isLoadingSignal.set(false);
      },
      error: () => {
        if (requestId !== this.requestSeq) return;
        this.readinessSignal.set(null);
        this.isLoadingSignal.set(false);
        this.hasErrorSignal.set(true);
      }
    });
  }

  getReadiness(engagementId: number): Observable<KickoffReadinessDto> {
    return this.http.get<KickoffReadinessDto>(`/api/Engagements/${engagementId}/kickoff-readiness`);
  }

  markKickoffReady(engagementId: number): Observable<void> {
    return this.http.post<void>(`/api/Engagements/${engagementId}/kickoff-ready`, {});
  }

  revertKickoffReady(engagementId: number): Observable<void> {
    return this.http.delete<void>(`/api/Engagements/${engagementId}/kickoff-ready`);
  }
}
