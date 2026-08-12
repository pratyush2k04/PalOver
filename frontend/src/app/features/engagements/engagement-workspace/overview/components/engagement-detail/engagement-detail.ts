import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EngagementService } from '../../../../services/engagement.service';
import { EngagementInfo, EngagementOwners, OwnerChange, OwnerKind } from '../../../../models/engagement.model';
import { UserDirectoryService } from '@features/users/services/user.service';
import { UserListItem } from '@features/users/models/user.model';
import { AuthService } from 'src/app/services/auth';
import { ToastService } from '@common/services/toast/toast';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { ENGAGEMENT_CONSTANTS, ENGAGEMENT_MESSAGES } from '@shared/constants/app.constants';

@Component({
  selector: 'app-engagement-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ConfirmDialogComponent, AlertComponent, ButtonComponent, SelectComponent],
  templateUrl: './engagement-detail.html',
  styleUrl: './engagement-detail.scss'
})
export class EngagementDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private engagementService = inject(EngagementService);
  private userDirectoryService = inject(UserDirectoryService);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);

  engagementId = Number(this.route.parent?.snapshot.paramMap.get('id') || this.route.snapshot.paramMap.get('id'));

  engagement = signal<EngagementInfo | null>(null);
  owners = signal<EngagementOwners | null>(null);
  canAssignOwners = this.authService.hasRole('Leadership');

  showAssignModal = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);
  projectManagers = signal<UserListItem[]>([]);
  techLeads = signal<UserListItem[]>([]);

  pendingChanges: OwnerChange[] = [];
  currentConfirmation = signal<OwnerChange | null>(null);
  isSavingConfirmation = signal(false);

  assignForm: FormGroup = this.fb.group({
    projectManagerId: [null, [Validators.required]],
    techLeadId: [null, [Validators.required]]
  });

  ngOnInit(): void {
    if (this.engagementId) {
      this.loadEngagement();
      this.loadOwners();
    }
  }

  private loadEngagement(): void {
    this.engagementService.getEngagementInfo(this.engagementId).subscribe({
      next: (info) => this.engagement.set(info)
    });
  }

  private loadOwners(): void {
    this.engagementService.getEngagementOwners(this.engagementId).subscribe({
      next: (owners) => this.owners.set(owners)
    });
  }

  get hasAnyOwnerAssigned(): boolean {
    const owners = this.owners();
    return !!owners && (!!owners.projectManagerId || !!owners.techLeadId);
  }

  get assignButtonLabel(): string {
    return this.hasAnyOwnerAssigned ? 'Reassign' : 'Assign';
  }

  readonly projectManagerOptions = computed<SelectOption[]>(() => [
    { value: null, label: '— Select —' },
    ...this.projectManagers().map((user) => ({ value: user.id, label: user.fullName }))
  ]);

  readonly techLeadOptions = computed<SelectOption[]>(() => [
    { value: null, label: '— Select —' },
    ...this.techLeads().map((user) => ({ value: user.id, label: user.fullName }))
  ]);

  openAssignModal(): void {
    if (!this.canAssignOwners) return;

    this.errorMessage.set(null);
    this.assignForm.reset({
      projectManagerId: this.owners()?.projectManagerId ?? null,
      techLeadId: this.owners()?.techLeadId ?? null
    });
    this.showAssignModal.set(true);

    this.userDirectoryService.getUsersInRole(ENGAGEMENT_CONSTANTS.PROJECT_MANAGEMENT_ROLE_ID).subscribe({
      next: (users) => this.projectManagers.set(users)
    });
    this.userDirectoryService.getUsersInRole(ENGAGEMENT_CONSTANTS.TECH_LEAD_ROLE_ID).subscribe({
      next: (users) => this.techLeads.set(users)
    });
  }

  closeAssignModal(): void {
    this.showAssignModal.set(false);
  }

  onSubmitAssignment(): void {
    if (this.assignForm.invalid) {
      this.assignForm.markAllAsTouched();
      return;
    }

    const { projectManagerId, techLeadId } = this.assignForm.value;
    const currentOwners = this.owners();

    this.pendingChanges = [];
    if (projectManagerId !== (currentOwners?.projectManagerId ?? null)) {
      this.pendingChanges.push({
        kind: 'projectManager',
        label: 'Project Manager',
        userId: projectManagerId,
        userName: this.projectManagers().find((u) => u.id === projectManagerId)?.fullName ?? ''
      });
    }
    if (techLeadId !== (currentOwners?.techLeadId ?? null)) {
      this.pendingChanges.push({
        kind: 'techLead',
        label: 'Tech / Delivery Lead',
        userId: techLeadId,
        userName: this.techLeads().find((u) => u.id === techLeadId)?.fullName ?? ''
      });
    }

    if (this.pendingChanges.length === 0) {
      this.showAssignModal.set(false);
      return;
    }

    this.processNextChange();
  }

  private processNextChange(): void {
    const change = this.pendingChanges.shift();
    if (!change) {
      this.isSubmitting.set(false);
      this.showAssignModal.set(false);
      return;
    }

    const isReassignment = this.isOwnerAlreadyAssigned(change.kind);
    const isPostKickoff = this.engagement()?.status !== ENGAGEMENT_CONSTANTS.KICKOFF_PENDING_STATUS;

    if (isReassignment && isPostKickoff) {
      this.currentConfirmation.set(change);
      return;
    }

    this.saveChange(change);
  }

  private isOwnerAlreadyAssigned(kind: OwnerKind): boolean {
    const owners = this.owners();
    return kind === 'projectManager' ? !!owners?.projectManagerId : !!owners?.techLeadId;
  }

  confirmPendingChange(): void {
    const change = this.currentConfirmation();
    if (!change) return;

    this.isSavingConfirmation.set(true);
    this.saveChange(change, () => {
      this.isSavingConfirmation.set(false);
      this.currentConfirmation.set(null);
    });
  }

  cancelPendingChange(): void {
    this.currentConfirmation.set(null);
    this.pendingChanges = [];
    this.isSubmitting.set(false);
  }

  private saveChange(change: OwnerChange, onSaved?: () => void): void {
    this.isSubmitting.set(true);
    const request$ =
      change.kind === 'projectManager'
        ? this.engagementService.assignProjectManager(this.engagementId, change.userId)
        : this.engagementService.assignTechLead(this.engagementId, change.userId);

    request$.subscribe({
      next: (owners) => {
        this.owners.set(owners);
        this.toastService.success(ENGAGEMENT_MESSAGES.ASSIGNMENT_SUCCESS(change.userName, change.label));
        onSaved?.();
        this.processNextChange();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.pendingChanges = [];
        onSaved?.();
        this.toastService.error(this.extractErrorMessage(err));
      }
    });
  }

  private extractErrorMessage(err: any): string {
    return typeof err.error === 'string' ? err.error : err.error?.message ?? ENGAGEMENT_MESSAGES.ASSIGNMENT_ERROR;
  }
}
