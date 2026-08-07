import { Component, OnInit, inject, signal } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EngagementInfo, EngagementOwners, EngagementService } from '../../services/engagement';
import { UserDirectoryService } from '../../services/user-directory';
import { UserListItem } from '../../models/user.model';
import { AuthService } from '../../services/auth';
import { ToastService } from '../../services/toast';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog';

const PROJECT_MANAGEMENT_ROLE_ID = 2;
const TECH_LEAD_ROLE_ID = 3;
const KICKOFF_PENDING_STATUS = 'KickoffPending';

type OwnerKind = 'projectManager' | 'techLead';

interface OwnerChange {
  kind: OwnerKind;
  label: string;
  userId: number;
  userName: string;
}

@Component({
  selector: 'app-engagement-detail',
  standalone: true,
  imports: [ReactiveFormsModule, ConfirmDialogComponent],
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

  engagementId = Number(this.route.snapshot.paramMap.get('id'));

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
    this.loadEngagement();
    this.loadOwners();
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

  openAssignModal(): void {
    if (!this.canAssignOwners) return;

    this.errorMessage.set(null);
    this.assignForm.reset({
      projectManagerId: this.owners()?.projectManagerId ?? null,
      techLeadId: this.owners()?.techLeadId ?? null
    });
    this.showAssignModal.set(true);

    this.userDirectoryService.getUsersInRole(PROJECT_MANAGEMENT_ROLE_ID).subscribe({
      next: (users) => this.projectManagers.set(users)
    });
    this.userDirectoryService.getUsersInRole(TECH_LEAD_ROLE_ID).subscribe({
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
    const isPostKickoff = this.engagement()?.status !== KICKOFF_PENDING_STATUS;

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
        this.toastService.success(`${change.userName} assigned as ${change.label}.`);
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
    return typeof err.error === 'string' ? err.error : err.error?.message ?? 'Failed to save assignment. Please try again.';
  }
}
