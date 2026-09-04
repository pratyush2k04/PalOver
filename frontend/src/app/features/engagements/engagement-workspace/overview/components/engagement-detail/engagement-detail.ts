import { Component, OnInit, computed, inject, signal, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EngagementService } from '../../../../services/engagement.service';
import { EngagementInfo, OwnerChange, OwnerKind } from '../../../../models/engagement.model';
import { UserDirectoryService } from '@features/users/services/user.service';
import { UserListItem } from '@features/users/models/user.model';
import { AuthService } from '@shared/services/auth';
import { ToastService } from '@common/services/toast/toast';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { ENGAGEMENT_CONSTANTS, ENGAGEMENT_MESSAGES, USER_ROLES } from '@shared/constants/app.constants';

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
  readonly owners = this.engagementService.owners;
  canAssignOwners = computed(() => this.authService.hasRole(USER_ROLES.LEADERSHIP) && !this.engagementService.isSignedOff());

  showAssignModal = signal(false);
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);
  projectManagers = signal<UserListItem[]>([]);
  techLeads = signal<UserListItem[]>([]);

  pendingChanges: OwnerChange[] = [];
  currentConfirmation = signal<OwnerChange | null>(null);

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
    this.engagementService.getEngagementOwners(this.engagementId).subscribe();
  }

  get hasAnyOwnerAssigned(): boolean {
    const owners = this.owners();
    return !!owners && (!!owners.projectManagerId || !!owners.techLeadId);
  }

  get assignButtonLabel(): string {
    return this.hasAnyOwnerAssigned ? 'Reassign' : 'Assign';
  }

  readonly projectManagerOptions = computed<SelectOption[]>(() =>
    this.projectManagers().map((user) => ({ value: user.id, label: user.fullName }))
  );

  readonly techLeadOptions = computed<SelectOption[]>(() =>
    this.techLeads().map((user) => ({ value: user.id, label: user.fullName }))
  );

  openAssignModal(): void {
    if (!this.canAssignOwners) return;

    this.errorMessage.set(null);
    this.isSubmitting.set(false);
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

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    if (this.showAssignModal()) {
      this.closeAssignModal();
    }
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
        label: 'Tech Lead',
        userId: techLeadId,
        userName: this.techLeads().find((u) => u.id === techLeadId)?.fullName ?? ''
      });
    }

    if (this.pendingChanges.length === 0) {
      this.showAssignModal.set(false);
      return;
    }

    this.errorMessage.set(null);
    this.processNextChange();
  }

  private processNextChange(): void {
    const change = this.pendingChanges.shift();
    if (!change) {
      return;
    }

    const isReassignment = this.isOwnerAlreadyAssigned(change.kind);
    const isPostKickoff = this.engagement()?.status !== ENGAGEMENT_CONSTANTS.KICKOFF_PENDING_STATUS;

    if (isReassignment && isPostKickoff) {
      this.showAssignModal.set(false);
      this.errorMessage.set(null);
      this.currentConfirmation.set(change);
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);
    this.saveChange(change);
  }

  private isOwnerAlreadyAssigned(kind: OwnerKind): boolean {
    const owners = this.owners();
    return kind === 'projectManager' ? !!owners?.projectManagerId : !!owners?.techLeadId;
  }

  confirmPendingChange(): void {
    const change = this.currentConfirmation();
    if (!change) return;

    this.errorMessage.set(null);
    this.isSubmitting.set(true);
    this.saveChange(change);
  }

  cancelPendingChange(): void {
    this.currentConfirmation.set(null);
    this.pendingChanges = [];
    this.errorMessage.set(null);
  }

  private saveChange(change: OwnerChange): void {
    const request$ =
      change.kind === 'projectManager'
        ? this.engagementService.assignProjectManager(this.engagementId, change.userId, true)
        : this.engagementService.assignTechLead(this.engagementId, change.userId, true);

    request$.subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.showAssignModal.set(false);
        this.currentConfirmation.set(null);
        this.toastService.success(ENGAGEMENT_MESSAGES.ASSIGNMENT_SUCCESS(change.userName, change.label));
        this.processNextChange();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(this.extractErrorMessage(err));
      }
    });
  }

  private extractErrorMessage(err: any): string {
    return typeof err.error === 'string' ? err.error : err.error?.message ?? ENGAGEMENT_MESSAGES.ASSIGNMENT_ERROR;
  }
}
