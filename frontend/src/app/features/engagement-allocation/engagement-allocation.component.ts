import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
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
  AssignLeadershipDialogData
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

  loadData(): void {
    this.loading.set(true);
    this.allocationService.getAllEngagements().subscribe({
      next: (data) => {
        const items: EngagementAllocationItem[] = data.map(e => ({
          ...e,
          leadershipUsers: [],
          isLoadingLeadership: true,
          hasHandoverDraft: false,
          isLoadingHandover: true
        }));
        this.engagements.set(items);
        this.loading.set(false);

        // Fetch leadership users for each engagement
        items.forEach(eng => {
          this.allocationService.getLeadershipUsers(eng.engagementId).subscribe({
            next: (users) => {
              this.engagements.update(list =>
                list.map(item =>
                  item.engagementId === eng.engagementId
                    ? { ...item, leadershipUsers: users, isLoadingLeadership: false }
                    : item
                )
              );
            },
            error: () => {
              this.engagements.update(list =>
                list.map(item =>
                  item.engagementId === eng.engagementId
                    ? { ...item, isLoadingLeadership: false }
                    : item
                )
              );
            }
          });
        });

        // Fetch handover draft status for each engagement
        items.forEach(eng => {
          this.allocationService.getHandoverDraftStatus(eng.engagementId).subscribe({
            next: (status) => {
              this.engagements.update(list =>
                list.map(item =>
                  item.engagementId === eng.engagementId
                    ? { ...item, hasHandoverDraft: status.hasHandoverDraft, isLoadingHandover: false }
                    : item
                )
              );
            },
            error: () => {
              this.engagements.update(list =>
                list.map(item =>
                  item.engagementId === eng.engagementId
                    ? { ...item, isLoadingHandover: false }
                    : item
                )
              );
            }
          });
        });
      },
      error: () => {
        this.toastService.error('Failed to load engagements.');
        this.loading.set(false);
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
    const dialogRef = this.dialog.open<AssignLeadershipDialogComponent, AssignLeadershipDialogData, LeadershipUserItem[]>(
      AssignLeadershipDialogComponent,
      { data: { engagement, availableLeadershipUsers: this.availableLeadershipUsers() } }
    );

    dialogRef.closed$.subscribe((updatedUsers) => {
      if (!updatedUsers) return;
      this.updateEngagementInList(engagement.engagementId, updatedUsers);
    });
  }

  private updateEngagementInList(engagementId: number, updatedUsers: LeadershipUserItem[]): void {
    this.engagements.update(list =>
      list.map(item =>
        item.engagementId === engagementId
          ? { ...item, leadershipUsers: updatedUsers }
          : item
      )
    );
  }
}
