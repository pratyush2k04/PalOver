import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '@shared/services/auth';
import { EngagementService } from '../../services/engagement.service';
import { EngagementChecklistService } from '@features/engagements/services/engagement-checklist.service';
import { EngagementChecklist, EngagementChecklistItem } from '../../models/engagement-checklist.model';
import { ENGAGEMENT_CHECKLIST_MESSAGES } from '@shared/constants/engagements.constants';
import { DialogService } from '@common/services/dialog/dialog.service';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { CommunicationCadenceService } from '@features/engagements/services/communication-cadence.service';
import { KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { EngagementParticipant } from '@features/engagements/models/cadence.model';
import {
  MarkNaDialogComponent,
  MarkNaDialogData,
  MarkNaDialogResult
} from './dialogs/mark-na-dialog/mark-na-dialog.component';
import {
  ExemptionReviewDialogComponent,
  ExemptionReviewDialogData,
  ExemptionReviewDialogResult
} from './dialogs/exemption-review-dialog/exemption-review-dialog.component';
import {
  BlockItemDialogComponent,
  BlockItemDialogData,
  BlockItemDialogResult
} from './dialogs/block-item-dialog/block-item-dialog.component';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import {
  AttachEvidenceDialogComponent,
  AttachEvidenceDialogData,
  AttachEvidenceDialogResult
} from './dialogs/attach-evidence-dialog/attach-evidence-dialog.component';

import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-engagement-checklist',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InlineSVGModule,
    StatusBadgeComponent,
    ButtonComponent,
    SectionHeaderComponent,
    TooltipDirective,
    SelectComponent,
  ],
  templateUrl: './engagement-checklist.component.html',
  styleUrl: './engagement-checklist.component.scss'
})
export class EngagementChecklistComponent {
  private route = inject(ActivatedRoute);
  private checklistService = inject(EngagementChecklistService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);
  private dialog = inject(DialogService);
  private cadenceService = inject(CommunicationCadenceService);
  private kickoffReadinessService = inject(KickoffReadinessService);

  private paramMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  checklist = signal<EngagementChecklist | null>(null);
  isLoading = signal<boolean>(true);
  isUpdating = signal<boolean>(false);
  error = signal<string | null>(null);
  assignedUsers = signal<EngagementParticipant[]>([]);

  // Search and filter signals
  searchTerm = signal<string>('');
  selectedCategoryId = signal<number | 'all'>('all');
  selectedStatus = signal<string>('all');
  selectedEvidenceFilter = signal<string>('all');
  selectedOwnerId = signal<number | 'all' | 'unassigned'>('all');

  // Role permissions & sign-off state
  readonly isSignedOff = computed(() => this.engagementService.isSignedOff());
  isProjectManager = computed(() => this.authService.hasRole('Project Management'));
  isLeadership = computed(() => this.authService.hasRole('Leadership'));
  isPreSales = computed(() => this.authService.hasRole('Pre Sales'));
  isTechLead = computed(() => this.authService.hasRole('Tech Lead'));

  canToggleStatus = computed(() => !this.isSignedOff() && (this.isProjectManager() || this.isTechLead()));
  canChangeApplicability = computed(() => !this.isSignedOff() && this.isProjectManager());
  canPerformAnyAction = computed(() => !this.isSignedOff() && (this.canToggleStatus() || this.canChangeApplicability() || this.isLeadership()));
  canLeadershipReview = computed(() => !this.isSignedOff() && this.isLeadership() && (this.checklist()?.pendingExemptionsCount ?? 0) > 0);
  canPerformNaAction = computed(() => !this.isSignedOff() && (this.canChangeApplicability() || this.canLeadershipReview()));

  categoriesList = computed(() => {
    return this.checklist()?.categories.map((c) => ({ id: c.categoryId, name: c.categoryName })) ?? [];
  });

  categoryFilterOptions = computed<SelectOption[]>(() => [
    { value: 'all', label: 'All Categories' },
    ...this.categoriesList().map((c) => ({ value: c.id, label: c.name }))
  ]);

  readonly statusFilterOptions: SelectOption[] = [
    { value: 'all', label: 'All Status' },
    { value: 'Incomplete', label: 'Incomplete' },
    { value: 'Complete', label: 'Complete' },
    { value: 'Blocked', label: 'Blocked' },
    { value: 'Pending', label: 'Pending Approval' }
  ];

  readonly evidenceFilterOptions: SelectOption[] = [
    { value: 'all', label: 'All Evidence' },
    { value: 'required', label: 'Evidence Required' },
    { value: 'attached', label: 'Evidence Attached' },
    { value: 'missing', label: 'Evidence Missing' }
  ];

  ownerFilterOptions = computed<SelectOption[]>(() => [
    { value: 'all', label: 'All Owners' },
    { value: 'unassigned', label: 'Unassigned' },
    ...this.assignedUsers().map((u) => ({ value: u.userId, label: u.fullName }))
  ]);

  rowOwnerOptions = computed<SelectOption[]>(() => [
    { value: null, label: 'Unassigned' },
    ...this.assignedUsers().map((u) => ({ value: u.userId, label: u.fullName }))
  ]);

  blockedGateItems = computed(() =>
    (this.checklist()?.incompleteMandatoryItems ?? []).filter((item) => item.status === 'Blocked')
  );
  incompleteGateItems = computed(() =>
    (this.checklist()?.incompleteMandatoryItems ?? []).filter((item) => item.status !== 'Blocked')
  );

  hasActiveFilters = computed(() => {
    return (
      this.searchTerm().trim() !== '' ||
      this.selectedCategoryId() !== 'all' ||
      this.selectedStatus() !== 'all' ||
      this.selectedEvidenceFilter() !== 'all' ||
      this.selectedOwnerId() !== 'all'
    );
  });

  filteredCategories = computed(() => {
    const cl = this.checklist();
    if (!cl) return [];

    const search = this.searchTerm().trim().toLowerCase();
    const catId = this.selectedCategoryId();
    const status = this.selectedStatus();
    const evidence = this.selectedEvidenceFilter();
    const owner = this.selectedOwnerId();

    return cl.categories
      .filter((category) => catId === 'all' || category.categoryId === Number(catId))
      .map((category) => {
        const filteredItems = category.items.filter((item) => {
          if (!item.isApplicable) return false;

          // Search term
          if (search) {
            const matchTitle = item.title?.toLowerCase().includes(search);
            const matchDesc = item.description?.toLowerCase().includes(search);
            if (!matchTitle && !matchDesc) return false;
          }

          // Status filter
          if (status !== 'all') {
            if (status === 'Pending') {
              if (item.exemptionStatus !== 'Pending') return false;
            } else if (item.status !== status) {
              return false;
            }
          }

          // Evidence filter
          if (evidence !== 'all') {
            const hasEvidence = !!item.evidenceNote || !!item.evidenceLink;
            if (evidence === 'required' && !item.evidenceRequired) return false;
            if (evidence === 'attached' && !hasEvidence) return false;
            if (evidence === 'missing' && (!item.evidenceRequired || hasEvidence)) return false;
          }

          // Owner filter
          if (owner !== 'all') {
            if (owner === 'unassigned') {
              if (item.ownerId != null) return false;
            } else {
              if (item.ownerId !== Number(owner)) return false;
            }
          }

          return true;
        });

        return {
          ...category,
          items: filteredItems
        };
      })
      .filter((category) => category.items.length > 0);
  });

  notApplicableItems = computed(() => {
    const cl = this.checklist();
    if (!cl) return [];
    return cl.categories.flatMap((cat) =>
      cat.items.filter((i) => !i.isApplicable).map((i) => ({ ...i, categoryName: cat.categoryName }))
    );
  });

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadChecklist(id);
        this.loadAssignedUsers(id);
      }
    });
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedCategoryId.set('all');
    this.selectedStatus.set('all');
    this.selectedEvidenceFilter.set('all');
    this.selectedOwnerId.set('all');
  }

  private loadChecklist(engagementId: number): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.checklistService.getEngagementChecklist(engagementId).subscribe({
      next: (data) => {
        this.checklist.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set(ENGAGEMENT_CHECKLIST_MESSAGES.LOAD_FAILED);
        this.isLoading.set(false);
      }
    });
  }

  private loadAssignedUsers(engagementId: number): void {
    this.cadenceService.getAssignedParticipants(engagementId).subscribe({
      next: (users) => this.assignedUsers.set(users ?? []),
      error: () => this.assignedUsers.set([])
    });
  }

  /**
   * Applies a mutation's returned checklist and, unless told otherwise, refreshes the
   * shared kickoff readiness score — the backend check only reads IsMandatory/IsApplicable/
   * Status, so owner-only changes should pass refreshReadiness: false to skip a wasted fetch.
   */
  private applyUpdatedChecklist(updatedChecklist: EngagementChecklist, opts: { refreshReadiness?: boolean } = {}): void {
    this.checklist.set(updatedChecklist);
    this.isUpdating.set(false);
    if (opts.refreshReadiness ?? true) {
      this.kickoffReadinessService.refresh();
    }
  }

  toggleItemStatus(item: EngagementChecklistItem): void {
    const engId = this.engagementId();
    if (!engId || !this.canToggleStatus() || !item.isApplicable || this.isUpdating()) {
      return;
    }

    // Intercept if completing an item with required evidence that hasn't been provided yet
    if (item.status !== 'Complete' && item.evidenceRequired && !item.evidenceNote && !item.evidenceLink) {
      this.openAttachEvidenceDialog(item, true);
      return;
    }

    const nextStatus = item.status === 'Complete' ? 'Incomplete' : 'Complete';
    this.isUpdating.set(true);

    this.checklistService.updateItemStatus(engId, item.id, { status: nextStatus }).subscribe({
      next: (updatedChecklist) => this.applyUpdatedChecklist(updatedChecklist),
      error: () => {
        this.isUpdating.set(false);
      }
    });
  }

  openAttachEvidenceDialog(item: EngagementChecklistItem, isMandatoryCompletion = false): void {
    const engId = this.engagementId();
    if (!engId || this.isUpdating()) {
      return;
    }

    const isReadOnly = !this.canToggleStatus();

    const dialogRef = this.dialog.open<
      AttachEvidenceDialogComponent,
      AttachEvidenceDialogData,
      AttachEvidenceDialogResult
    >(AttachEvidenceDialogComponent, {
      data: {
        item,
        isMandatoryCompletion,
        isReadOnly
      }
    });

    dialogRef.closed$.subscribe((result) => {
      if (result && result.note) {
        this.isUpdating.set(true);
        this.checklistService.saveItemEvidence(engId, item.id, {
          note: result.note,
          link: result.link,
          markComplete: result.markComplete
        }).subscribe({
          next: (updatedChecklist) => this.applyUpdatedChecklist(updatedChecklist),
          error: () => {
            this.isUpdating.set(false);
          }
        });
      }
    });
  }

  openMarkNaDialog(item: EngagementChecklistItem): void {
    const engId = this.engagementId();
    if (!engId || !this.canChangeApplicability() || this.isUpdating()) {
      return;
    }

    const dialogRef = this.dialog.open<MarkNaDialogComponent, MarkNaDialogData, MarkNaDialogResult>(
      MarkNaDialogComponent,
      {
        data: {
          itemTitle: item.title,
          isMandatory: item.isMandatory
        }
      }
    );

    dialogRef.closed$.subscribe((result) => {
      if (result && result.reason) {
        this.isUpdating.set(true);
        this.checklistService.updateItemApplicability(engId, item.id, {
          isApplicable: false,
          reason: result.reason
        }).subscribe({
          next: (updatedChecklist) => this.applyUpdatedChecklist(updatedChecklist),
          error: () => {
            this.isUpdating.set(false);
          }
        });
      }
    });
  }

  makeItemApplicable(item: EngagementChecklistItem): void {
    const engId = this.engagementId();
    if (!engId || !this.canChangeApplicability() || this.isUpdating()) {
      return;
    }

    this.isUpdating.set(true);
    this.checklistService.updateItemApplicability(engId, item.id, {
      isApplicable: true
    }).subscribe({
      next: (updatedChecklist) => this.applyUpdatedChecklist(updatedChecklist),
      error: () => {
        this.isUpdating.set(false);
      }
    });
  }

  openReviewDialog(item: EngagementChecklistItem): void {
    const engId = this.engagementId();
    if (!engId || !this.isLeadership() || item.exemptionStatus !== 'Pending' || this.isUpdating()) {
      return;
    }

    const dialogRef = this.dialog.open<
      ExemptionReviewDialogComponent,
      ExemptionReviewDialogData,
      ExemptionReviewDialogResult
    >(ExemptionReviewDialogComponent, {
      data: {
        itemTitle: item.title,
        reason: item.notApplicableReason || '',
        requestedAt: item.exemptionRequestedAt
      }
    });

    dialogRef.closed$.subscribe((result) => {
      if (result !== undefined) {
        this.isUpdating.set(true);
        this.checklistService.reviewExemption(engId, item.id, {
          approved: result.approved,
          rejectionReason: result.rejectionReason
        }).subscribe({
          next: (updatedChecklist) => this.applyUpdatedChecklist(updatedChecklist),
          error: () => {
            this.isUpdating.set(false);
          }
        });
      }
    });
  }

  onOwnerChange(item: EngagementChecklistItem, personIdValue: unknown): void {
    const engId = this.engagementId();
    if (!engId || !this.canToggleStatus() || this.isUpdating()) {
      return;
    }

    const personId = (personIdValue === '' || personIdValue === null || personIdValue === undefined)
      ? null
      : Number(personIdValue);
    this.isUpdating.set(true);

    this.checklistService.updateItemOwner(engId, item.id, { personId }).subscribe({
      next: (updatedChecklist) => this.applyUpdatedChecklist(updatedChecklist, { refreshReadiness: false }),
      error: () => {
        this.isUpdating.set(false);
      }
    });
  }

  openBlockDialog(item: EngagementChecklistItem): void {
    const engId = this.engagementId();
    if (!engId || !this.canToggleStatus() || !item.isApplicable || item.status === 'Blocked' || this.isUpdating()) {
      return;
    }

    const dialogRef = this.dialog.open<BlockItemDialogComponent, BlockItemDialogData, BlockItemDialogResult>(
      BlockItemDialogComponent,
      {
        data: {
          itemTitle: item.title,
          currentOwnerId: item.ownerId,
          assignedUsers: this.assignedUsers()
        }
      }
    );

    dialogRef.closed$.subscribe((result) => {
      if (result) {
        this.isUpdating.set(true);
        this.checklistService.blockItem(engId, item.id, {
          reason: result.reason,
          personId: result.personId
        }).subscribe({
          next: (updatedChecklist) => this.applyUpdatedChecklist(updatedChecklist),
          error: () => {
            this.isUpdating.set(false);
          }
        });
      }
    });
  }

  unblockItem(item: EngagementChecklistItem): void {
    const engId = this.engagementId();
    if (!engId || !this.canToggleStatus() || item.status !== 'Blocked' || this.isUpdating()) {
      return;
    }

    this.isUpdating.set(true);
    this.checklistService.unblockItem(engId, item.id).subscribe({
      next: (updatedChecklist) => this.applyUpdatedChecklist(updatedChecklist),
      error: () => {
        this.isUpdating.set(false);
      }
    });
  }
}
