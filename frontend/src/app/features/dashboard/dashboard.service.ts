import { Injectable, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../../services/auth';
import { ChecklistService } from '../checklist/checklist.service';
import { ChecklistCategory, ChecklistVersion } from '../checklist/checklist.model';
import { DashboardSummary } from './dashboard.model';

// Mocked until the backend exposes an activity/audit log and change-tracking for drafts.
const MOCK_PENDING_DRAFT_CHANGES = 5;
const MOCK_RECENT_ACTIVITY: DashboardSummary['recentActivity'] = [
  { id: '1', description: 'Published version v2.4', timestamp: '2026-07-24', actor: 'Meera Krishnan' },
  { id: '2', description: 'Added item "Data Residency Review"', timestamp: '2026-07-22', actor: 'Arjun Rao' },
  { id: '3', description: 'Retired item "Legacy SLA Clause"', timestamp: '2026-07-20', actor: 'Meera Krishnan' },
];
const MOCK_RECENT_CHECKLIST_UPDATES: DashboardSummary['recentChecklistUpdates'] = [
  { id: '1', itemName: 'Data Residency Review', category: 'Compliance', updatedAt: '2026-07-22' },
  { id: '2', itemName: 'Security Questionnaire', category: 'Security', updatedAt: '2026-07-21' },
  { id: '3', itemName: 'Legacy SLA Clause', category: 'Contracts', updatedAt: '2026-07-20' },
];

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private authService = inject(AuthService);
  private checklistService = inject(ChecklistService);

  private readonly loadingSignal = signal(false);
  private readonly errorSignal = signal<string | null>(null);
  private readonly summarySignal = signal<DashboardSummary | null>(null);

  readonly loading = this.loadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly summary = this.summarySignal.asReadonly();

  loadSummary(): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    const isAdmin = this.authService.isAdmin();

    forkJoin({
      published: this.checklistService.fetchPublished().pipe(catchError(() => of(null))),
      draft: isAdmin
        ? this.checklistService.fetchCurrentDraft().pipe(catchError(() => of(null)))
        : of(null),
      categories: isAdmin
        ? this.checklistService.fetchCategories().pipe(catchError(() => of<ChecklistCategory[]>([])))
        : of<ChecklistCategory[]>([]),
    }).subscribe(({ published, draft, categories }) => {
      const activeVersion: ChecklistVersion | null = draft ?? published;
      const activeItems = (activeVersion?.items ?? []).filter((item) => !item.isRetired);

      this.summarySignal.set({
        latestPublishedVersion: published ? { versionLabel: `v${published.versionNumber}` } : null,
        pendingDraftChangesCount: MOCK_PENDING_DRAFT_CHANGES,
        quickStatistics: {
          totalItems: activeItems.length,
          totalCategories: isAdmin ? categories.length : null,
          activeVersion: activeVersion ? `v${activeVersion.versionNumber}` : '—',
        },
        recentActivity: MOCK_RECENT_ACTIVITY,
        recentChecklistUpdates: MOCK_RECENT_CHECKLIST_UPDATES,
      });
      this.loadingSignal.set(false);
    });
  }
}
