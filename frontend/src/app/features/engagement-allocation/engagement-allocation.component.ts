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

@Component({
  selector: 'app-engagement-allocation',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent, InlineSVGModule, StatusBadgeComponent, ButtonComponent],
  templateUrl: './engagement-allocation.component.html',
  styleUrl: './engagement-allocation.component.scss'
})
export class EngagementAllocationComponent implements OnInit {
  private allocationService = inject(EngagementAllocationService);
  private toastService = inject(ToastService);

  readonly engagements = signal<EngagementAllocationItem[]>([]);
  readonly availableLeadershipUsers = signal<LeadershipUserItem[]>([]);
  readonly loading = signal<boolean>(true);
  readonly searchQuery = signal<string>('');
  readonly filterMode = signal<'all' | 'needing'>('all');

  // Modal State
  readonly isModalOpen = signal<boolean>(false);
  readonly selectedEngagement = signal<EngagementAllocationItem | null>(null);
  readonly modalLeadershipUsers = signal<LeadershipUserItem[]>([]);
  readonly modalSearchTerm = signal<string>('');
  readonly isDropdownOpen = signal<boolean>(false);
  readonly isSaving = signal<boolean>(false);

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

  // Filtered Available Users for Dropdown in Modal
  readonly filteredAvailableUsers = computed(() => {
    const term = this.modalSearchTerm().toLowerCase().trim();
    const assignedIds = new Set(this.modalLeadershipUsers().map(u => u.id));

    return this.availableLeadershipUsers().filter(u => {
      const isNotAlreadyAssigned = !assignedIds.has(u.id);
      const matchesTerm =
        !term ||
        u.fullName.toLowerCase().includes(term) ||
        u.id.toString().includes(term) ||
        u.email.toLowerCase().includes(term);

      return isNotAlreadyAssigned && matchesTerm;
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
          isLoadingLeadership: true
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

  openAssignModal(engagement: EngagementAllocationItem): void {
    this.selectedEngagement.set(engagement);
    this.modalLeadershipUsers.set([...(engagement.leadershipUsers || [])]);
    this.modalSearchTerm.set('');
    this.isDropdownOpen.set(false);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.selectedEngagement.set(null);
    this.modalLeadershipUsers.set([]);
    this.modalSearchTerm.set('');
    this.isDropdownOpen.set(false);
  }

  onSearchFocus(): void {
    this.isDropdownOpen.set(true);
  }

  onSearchBlur(): void {
    // Delay hiding dropdown so click events on dropdown items can register
    setTimeout(() => {
      this.isDropdownOpen.set(false);
    }, 200);
  }

  selectUserFromDropdown(user: LeadershipUserItem): void {
    const eng = this.selectedEngagement();
    if (!eng) return;

    // Check if already in list
    if (this.modalLeadershipUsers().some(u => u.id === user.id)) {
      this.toastService.error('This user is already assigned to this engagement');
      return;
    }

    this.isSaving.set(true);
    this.allocationService.assignLeadershipUser(eng.engagementId, user.id).subscribe({
      next: (updatedUsers) => {
        this.modalLeadershipUsers.set(updatedUsers);
        this.updateEngagementInList(eng.engagementId, updatedUsers);
        this.toastService.success(`Assigned ${user.fullName} to ${eng.engagementName}`);
        this.modalSearchTerm.set('');
        this.isSaving.set(false);
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'Failed to assign Leadership user.';
        this.toastService.error(errorMsg);
        this.isSaving.set(false);
      }
    });
  }

  removeTagChip(user: LeadershipUserItem): void {
    const eng = this.selectedEngagement();
    if (!eng) return;

    if (this.modalLeadershipUsers().length <= 1) {
      this.toastService.error('At least one Leadership contact is required for this engagement.');
      return;
    }

    this.isSaving.set(true);
    this.allocationService.removeLeadershipUser(eng.engagementId, user.id).subscribe({
      next: (updatedUsers) => {
        this.modalLeadershipUsers.set(updatedUsers);
        this.updateEngagementInList(eng.engagementId, updatedUsers);
        this.toastService.success(`Removed ${user.fullName} from ${eng.engagementName}`);
        this.isSaving.set(false);
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'Failed to remove Leadership user.';
        this.toastService.error(errorMsg);
        this.isSaving.set(false);
      }
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
