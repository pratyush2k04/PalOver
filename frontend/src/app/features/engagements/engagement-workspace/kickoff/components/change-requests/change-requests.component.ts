import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { ChangeRequestService } from '@features/engagements/services/change-request.service';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { AuthService } from '@shared/services/auth';
import { ToastService } from '@common/services/toast/toast';
import { ChangeRequest } from '@features/engagements/models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/change-request.constants';
import { USER_ROLES } from '@shared/constants/users.constants';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { DialogService } from '@common/services/dialog/dialog.service';
import { LogCrDialogComponent, LogCrDialogData } from './dialogs/log-cr-dialog/log-cr-dialog.component';
import { RejectCrDialogComponent, RejectCrDialogData } from './dialogs/reject-cr-dialog/reject-cr-dialog.component';
import { ViewReasonDialogComponent, ViewReasonDialogData } from './dialogs/view-reason-dialog/view-reason-dialog.component';

@Component({
  selector: 'app-change-requests',
  standalone: true,
  imports: [
    CommonModule,
    InlineSVGModule,
    StatusBadgeComponent,
    ButtonComponent,
    LoadingSpinnerComponent,
    TooltipDirective
  ],
  templateUrl: './change-requests.component.html',
  styleUrl: './change-requests.component.scss'
})
export class ChangeRequestsComponent {
  private route = inject(ActivatedRoute);
  private changeRequestService = inject(ChangeRequestService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);
  private dialog = inject(DialogService);
  private toastService = inject(ToastService);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  readonly isSignedOff = computed(() => this.engagementService.isSignedOff());
  isProjectManager = computed(() => !this.isSignedOff() && this.engagementService.isWorkspacePm());
  isLeadership = computed(() => !this.isSignedOff() && (this.engagementService.isWorkspaceLeadership() || this.authService.isAdmin()));

  private paramMap = toSignal(this.route.paramMap);
  private parentParamMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id') || this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  changeRequests = signal<ChangeRequest[]>([]);
  selectedStatusFilter = signal<string>('All');
  isLoading = signal(false);

  filteredChangeRequests = computed(() => {
    const filter = this.selectedStatusFilter();
    const list = this.changeRequests();
    if (filter === 'All') return list;
    return list.filter(item => item.status === filter);
  });

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadChangeRequests(id);
      }
    });
  }

  loadChangeRequests(engagementId: number, onDone?: () => void): void {
    this.isLoading.set(true);
    this.changeRequestService.getChangeRequests(engagementId).subscribe({
      next: (data) => {
        this.changeRequests.set(data);
        this.isLoading.set(false);
        onDone?.();
      },
      error: (err) => {
        console.error('Failed to load change requests:', err);
        this.isLoading.set(false);
        onDone?.();
      }
    });
  }

  setFilter(status: string): void {
    this.selectedStatusFilter.set(status);
  }

  getStatusTone(status: string): StatusBadgeTone {
    switch (status) {
      case 'Approved':
        return 'success';
      case 'Rejected':
        return 'danger';
      default:
        return 'info';
    }
  }

  // --- Log Change Request ---
  openLogModal(): void {
    const id = this.engagementId();
    if (!id || !this.isProjectManager() || this.isLoading()) return;

    const dialogRef = this.dialog.open<LogCrDialogComponent, LogCrDialogData, boolean>(
      LogCrDialogComponent,
      { data: { engagementId: id } }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.loadChangeRequests(id, () => this.toastService.success(this.constants.MSG_LOG_SUCCESS));
    });
  }

  // --- Approve Change Request ---
  onApprove(cr: ChangeRequest): void {
    if (!this.isLeadership() || cr.status !== 'Under Review') return;

    this.changeRequestService.approveChangeRequest(cr.id).subscribe({
      next: (updatedCr) => {
        this.changeRequests.update(list =>
          list.map(item => item.id === updatedCr.id ? updatedCr : item)
        );
      },
      error: (err) => {
        console.error('Failed to approve change request:', err);
      }
    });
  }

  // --- Reject Change Request ---
  openRejectModal(cr: ChangeRequest): void {
    const id = this.engagementId();
    if (!id || !this.isLeadership() || cr.status !== 'Under Review' || this.isLoading()) return;

    const dialogRef = this.dialog.open<RejectCrDialogComponent, RejectCrDialogData, boolean>(
      RejectCrDialogComponent,
      { data: { cr } }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.loadChangeRequests(id, () => this.toastService.success(this.constants.MSG_REJECT_SUCCESS));
    });
  }

  // --- View Rejection Reason ---
  openReasonModal(reason?: string): void {
    if (!reason) return;
    this.dialog.open<ViewReasonDialogComponent, ViewReasonDialogData, void>(
      ViewReasonDialogComponent,
      { data: { reason } }
    );
  }
}
