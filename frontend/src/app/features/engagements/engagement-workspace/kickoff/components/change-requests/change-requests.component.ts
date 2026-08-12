import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { ChangeRequestService } from '@features/engagements/services/change-request.service';
import { AuthService } from 'src/app/services/auth';
import { ChangeRequest } from '@features/engagements/models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/change-request.constants';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
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
    ButtonComponent
  ],
  templateUrl: './change-requests.component.html',
  styleUrl: './change-requests.component.scss'
})
export class ChangeRequestsComponent {
  private route = inject(ActivatedRoute);
  private changeRequestService = inject(ChangeRequestService);
  private authService = inject(AuthService);
  private dialog = inject(DialogService);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  isProjectManager = computed(() => this.authService.hasRole('Project Management'));
  isLeadership = computed(() => this.authService.hasRole('Leadership'));

  private paramMap = toSignal(this.route.paramMap);
  private parentParamMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id') || this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  changeRequests = signal<ChangeRequest[]>([]);
  selectedStatusFilter = signal<string>('All');

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

  loadChangeRequests(engagementId: number): void {
    this.changeRequestService.getChangeRequests(engagementId).subscribe({
      next: (data) => this.changeRequests.set(data),
      error: (err) => console.error('Failed to load change requests:', err)
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
    if (!id || !this.isProjectManager()) return;

    const dialogRef = this.dialog.open<LogCrDialogComponent, LogCrDialogData, ChangeRequest>(
      LogCrDialogComponent,
      { data: { engagementId: id } }
    );

    dialogRef.closed$.subscribe((newCr) => {
      if (!newCr) return;
      this.changeRequests.update(list => [newCr, ...list]);
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
    if (!this.isLeadership() || cr.status !== 'Under Review') return;

    const dialogRef = this.dialog.open<RejectCrDialogComponent, RejectCrDialogData, ChangeRequest>(
      RejectCrDialogComponent,
      { data: { cr } }
    );

    dialogRef.closed$.subscribe((updatedCr) => {
      if (!updatedCr) return;
      this.changeRequests.update(list =>
        list.map(item => item.id === updatedCr.id ? updatedCr : item)
      );
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
