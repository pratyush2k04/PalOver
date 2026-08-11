import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { ChangeRequestService } from '@features/engagements/services/change-request.service';
import { AuthService } from 'src/app/services/auth';
import { ChangeRequest } from '@features/engagements/models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from 'src/app/constants/change-request.constants';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AlertComponent } from '@common/components/alert/alert';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';

@Component({
  selector: 'app-change-requests',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InlineSVGModule,
    AlertComponent,
    StatusBadgeComponent,
    ButtonComponent,
    InputComponent,
    TextareaComponent
  ],
  templateUrl: './change-requests.component.html',
  styleUrl: './change-requests.component.scss'
})
export class ChangeRequestsComponent {
  private route = inject(ActivatedRoute);
  private changeRequestService = inject(ChangeRequestService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

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

  // Modal States
  isLogModalOpen = signal<boolean>(false);
  isRejectModalOpen = signal<boolean>(false);
  isReasonModalOpen = signal<boolean>(false);

  selectedCrForReject = signal<ChangeRequest | null>(null);
  selectedReasonText = signal<string | null>(null);

  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  // Forms
  logForm: FormGroup = this.fb.group({
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    impact: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  rejectForm: FormGroup = this.fb.group({
    rejectionReason: ['', [Validators.required, Validators.maxLength(2000)]]
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
    if (!this.isProjectManager()) return;

    this.errorMessage.set(null);
    this.logForm.reset({
      description: '',
      impact: ''
    });
    this.isLogModalOpen.set(true);
  }

  closeLogModal(): void {
    this.isLogModalOpen.set(false);
    this.errorMessage.set(null);
  }

  onSubmitLog(): void {
    if (this.logForm.invalid) {
      this.logForm.markAllAsTouched();
      return;
    }

    const id = this.engagementId();
    if (!id || !this.isProjectManager()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const payload = {
      description: this.logForm.value.description,
      impact: this.logForm.value.impact
    };

    this.changeRequestService.createChangeRequest(id, payload).subscribe({
      next: (newCr) => {
        this.isSubmitting.set(false);
        this.changeRequests.update(list => [newCr, ...list]);
        this.closeLogModal();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const backendMessage = err?.error?.message || err?.error || this.constants.ERROR_GENERIC;
        this.errorMessage.set(backendMessage);
      }
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

    this.selectedCrForReject.set(cr);
    this.errorMessage.set(null);
    this.rejectForm.reset({ rejectionReason: '' });
    this.isRejectModalOpen.set(true);
  }

  closeRejectModal(): void {
    this.isRejectModalOpen.set(false);
    this.selectedCrForReject.set(null);
    this.errorMessage.set(null);
  }

  onSubmitReject(): void {
    if (this.rejectForm.invalid) {
      this.rejectForm.markAllAsTouched();
      return;
    }

    const cr = this.selectedCrForReject();
    if (!cr || !this.isLeadership()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const payload = {
      rejectionReason: this.rejectForm.value.rejectionReason
    };

    this.changeRequestService.rejectChangeRequest(cr.id, payload).subscribe({
      next: (updatedCr) => {
        this.isSubmitting.set(false);
        this.changeRequests.update(list =>
          list.map(item => item.id === updatedCr.id ? updatedCr : item)
        );
        this.closeRejectModal();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const backendMessage = err?.error?.message || err?.error || this.constants.ERROR_GENERIC;
        this.errorMessage.set(backendMessage);
      }
    });
  }

  // --- View Rejection Reason ---
  openReasonModal(reason?: string): void {
    if (!reason) return;
    this.selectedReasonText.set(reason);
    this.isReasonModalOpen.set(true);
  }

  closeReasonModal(): void {
    this.isReasonModalOpen.set(false);
    this.selectedReasonText.set(null);
  }
}
