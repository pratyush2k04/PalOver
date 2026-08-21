import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '@shared/services/auth';
import { EngagementChecklistService } from '@features/engagements/services/engagement-checklist.service';
import { EngagementChecklist, EngagementChecklistItem } from '../../models/engagement-checklist.model';
import { ENGAGEMENT_CHECKLIST_MESSAGES } from '@shared/constants/engagements.constants';
import { USER_ROLES } from '@shared/constants/app.constants';
import { DialogService } from '@common/services/dialog/dialog.service';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';
import { CommunicationCadenceService } from '@features/engagements/services/communication-cadence.service';
import { EngagementParticipant } from '@features/engagements/models/cadence.model';
import { InlineSVGModule } from 'ng-inline-svg-2';
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

@Component({
  selector: 'app-engagement-checklist',
  standalone: true,
  imports: [CommonModule, InlineSVGModule, LoadingSpinnerComponent, StatusBadgeComponent, ButtonComponent, SectionHeaderComponent],
  templateUrl: './engagement-checklist.component.html',
  styleUrl: './engagement-checklist.component.scss'
})
export class EngagementChecklistComponent {
  private route = inject(ActivatedRoute);
  private checklistService = inject(EngagementChecklistService);
  private authService = inject(AuthService);
  private dialog = inject(DialogService);
  private cadenceService = inject(CommunicationCadenceService);

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

  // Role permissions
  isProjectManager = computed(() => this.authService.hasRole(USER_ROLES.PROJECT_MANAGEMENT) || this.authService.isAdmin());
  isLeadership = computed(() => this.authService.hasRole(USER_ROLES.LEADERSHIP) || this.authService.isAdmin());
  isPreSales = computed(() => this.authService.hasRole(USER_ROLES.PRE_SALES));
  isTechLead = computed(() => this.authService.hasRole(USER_ROLES.TECH_LEAD));

  canToggleStatus = computed(() => this.isProjectManager() || this.isTechLead() || this.isPreSales() || this.authService.isAdmin());
  canChangeApplicability = computed(() => this.isProjectManager() || this.isTechLead() || this.authService.isAdmin());
  canPerformAnyAction = computed(() => this.canToggleStatus() || this.canChangeApplicability() || this.isLeadership());

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

  toggleItemStatus(item: EngagementChecklistItem): void {
    const engId = this.engagementId();
    if (!engId || !this.canToggleStatus() || !item.isApplicable || this.isUpdating()) {
      return;
    }

    const nextStatus = item.status === 'Complete' ? 'Incomplete' : 'Complete';
    this.isUpdating.set(true);

    this.checklistService.updateItemStatus(engId, item.id, { status: nextStatus }).subscribe({
      next: (updatedChecklist) => {
        this.checklist.set(updatedChecklist);
        this.isUpdating.set(false);
      },
      error: () => {
        this.isUpdating.set(false);
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
          next: (updatedChecklist) => {
            this.checklist.set(updatedChecklist);
            this.isUpdating.set(false);
          },
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
      next: (updatedChecklist) => {
        this.checklist.set(updatedChecklist);
        this.isUpdating.set(false);
      },
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
          next: (updatedChecklist) => {
            this.checklist.set(updatedChecklist);
            this.isUpdating.set(false);
          },
          error: () => {
            this.isUpdating.set(false);
          }
        });
      }
    });
  }

  onOwnerChange(item: EngagementChecklistItem, personIdStr: string): void {
    const engId = this.engagementId();
    if (!engId || !this.canToggleStatus() || this.isUpdating()) {
      return;
    }

    const personId = personIdStr === '' ? null : Number(personIdStr);
    this.isUpdating.set(true);

    this.checklistService.updateItemOwner(engId, item.id, { personId }).subscribe({
      next: (updatedChecklist) => {
        this.checklist.set(updatedChecklist);
        this.isUpdating.set(false);
      },
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
          next: (updatedChecklist) => {
            this.checklist.set(updatedChecklist);
            this.isUpdating.set(false);
          },
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
      next: (updatedChecklist) => {
        this.checklist.set(updatedChecklist);
        this.isUpdating.set(false);
      },
      error: () => {
        this.isUpdating.set(false);
      }
    });
  }
}
