import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { EngagementAllocationService } from './services/engagement-allocation.service';
import { EngagementAllocationItem, LeadershipUserItem } from './models/engagement-allocation.model';
import { ToastService } from '@common/services/toast/toast';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { PageHeaderComponent } from '@common/components/page-header/page-header';
import { DialogService } from '@common/services/dialog/dialog.service';
import {
  AssignLeadershipDialogComponent,
  AssignLeadershipDialogData,
  AssignLeadershipDialogResult
} from './dialogs/assign-leadership-dialog/assign-leadership-dialog.component';

@Component({
  selector: 'app-engagement-allocation',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LoadingSpinnerComponent,
    InlineSVGModule,
    StatusBadgeComponent,
    ButtonComponent,
    PageHeaderComponent
  ],
  templateUrl: './engagement-allocation.component.html',
  styleUrl: './engagement-allocation.component.scss'
})
export class EngagementAllocationComponent implements OnInit {
  private allocationService = inject(EngagementAllocationService);
  private toastService = inject(ToastService);
  private dialog = inject(DialogService);

  readonly engagements = signal<EngagementAllocationItem[]>([]);
  readonly availableLeadershipUsers = signal<LeadershipUserItem[]>([]);
  readonly loading = signal<boolean>(true);
  readonly searchQuery = signal<string>('');
  readonly filterMode = signal<'all' | 'needing'>('all');

  // Metrics
  readonly totalCount = computed(() => this.engagements().length);
  readonly needingCount = computed(() =>
    this.engagements().filter(e => !e.leadershipUsers || e.leadershipUsers.length === 0).length
  );
  readonly allocatedCount = computed(() =>
    this.engagements().filter(e => e.leadershipUsers && e.leadershipUsers.length > 0).length
  );

  // Filtered Engagements
  readonly filteredEngagements = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const mode = this.filterMode();

    return this.engagements().filter(e => {
      const matchesSearch =
        !query ||
        e.engagementName.toLowerCase().includes(query) ||
        e.clientName.toLowerCase().includes(query) ||
        e.engagementType.toLowerCase().includes(query);

      const hasLeadership = e.leadershipUsers && e.leadershipUsers.length > 0;
      const matchesFilter = mode === 'all' || (mode === 'needing' && !hasLeadership);

      return matchesSearch && matchesFilter;
    });
  });

  ngOnInit(): void {
    this.loadData();
  }

  loadData(onDone?: () => void): void {
    this.loading.set(true);
    this.allocationService.getAllEngagements().subscribe({
      next: (data) => {
        const items: EngagementAllocationItem[] = data.map(e => ({
          ...e,
          leadershipUsers: [],
          hasHandoverDraft: false
        }));

        if (items.length === 0) {
          this.engagements.set(items);
          this.loading.set(false);
          onDone?.();
          return;
        }

        const leadershipRequests = items.map(eng =>
          this.allocationService.getLeadershipUsers(eng.engagementId).pipe(
            map(users => ({ engagementId: eng.engagementId, users })),
            catchError(() => of({ engagementId: eng.engagementId, users: [] as LeadershipUserItem[] }))
          )
        );

        const handoverRequests = items.map(eng =>
          this.allocationService.getHandoverDraftStatus(eng.engagementId).pipe(
            map(status => ({
              engagementId: eng.engagementId,
              hasHandoverDraft: !!status.hasHandoverDraft && status.status?.toLowerCase() === 'submitted'
            })),
            catchError(() => of({ engagementId: eng.engagementId, hasHandoverDraft: false }))
          )
        );

        forkJoin([forkJoin(leadershipRequests), forkJoin(handoverRequests)]).subscribe(([leadershipResults, handoverResults]) => {
          const leadershipMap = new Map(leadershipResults.map(r => [r.engagementId, r.users]));
          const handoverMap = new Map(handoverResults.map(r => [r.engagementId, r.hasHandoverDraft]));

          this.engagements.set(items.map(item => ({
            ...item,
            leadershipUsers: leadershipMap.get(item.engagementId) ?? [],
            hasHandoverDraft: handoverMap.get(item.engagementId) ?? false
          })));
          this.loading.set(false);
          onDone?.();
        });
      },
      error: () => {
        this.loading.set(false);
        this.toastService.error('Failed to load engagements.');
        onDone?.();
      }
    });

    // Fetch available leadership users
    this.allocationService.getAvailableLeadershipUsers().subscribe({
      next: (users) => {
        this.availableLeadershipUsers.set(users);
      },
      error: () => {
        this.toastService.error('Failed to load available Leadership users.');
      }
    });
  }

  setFilterMode(mode: 'all' | 'needing'): void {
    this.filterMode.set(mode);
  }

  getTypeTone(type: string): 'info' | 'neutral' | 'success' | 'danger' | 'warning' {
    if (!type) return 'neutral';
    const t = type.toLowerCase();
    if (t.includes('greenfield') || t.includes('green')) return 'success';
    if (t.includes('brownfield') || t.includes('brown')) return 'warning';
    if (t.includes('fixed') || t.includes('fp')) return 'info';
    if (t.includes('managed') || t.includes('ms')) return 'success';
    if (t.includes('consulting') || t.includes('advisory')) return 'danger';
    return 'neutral';
  }

  getStatusTone(status: string): 'info' | 'neutral' | 'success' | 'danger' | 'warning' {
    if (!status) return 'neutral';
    const s = status.toLowerCase();
    if (s.includes('active') || s.includes('completed') || s.includes('ready')) return 'success';
    if (s.includes('hold') || s.includes('pause') || s.includes('closed')) return 'danger';
    if (s.includes('pending') || s.includes('progress') || s.includes('draft')) return 'warning';
    return 'info';
  }

  openAssignModal(engagement: EngagementAllocationItem): void {
    const dialogRef = this.dialog.open<AssignLeadershipDialogComponent, AssignLeadershipDialogData, AssignLeadershipDialogResult>(
      AssignLeadershipDialogComponent,
      { data: { engagement, availableLeadershipUsers: this.availableLeadershipUsers() } }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;

      this.loadData(() => {
        const { addedNames, removedNames } = result;

        if (addedNames.length > 0 && removedNames.length === 0) {
          this.toastService.success(`Assigned ${addedNames.join(', ')} to ${engagement.engagementName}`);
        } else if (removedNames.length > 0 && addedNames.length === 0) {
          this.toastService.success(`Removed ${removedNames.join(', ')} from ${engagement.engagementName}`);
        } else if (addedNames.length > 0 && removedNames.length > 0) {
          this.toastService.success(`Updated leadership contacts for ${engagement.engagementName}`);
        }
      });
    });
  }
}
