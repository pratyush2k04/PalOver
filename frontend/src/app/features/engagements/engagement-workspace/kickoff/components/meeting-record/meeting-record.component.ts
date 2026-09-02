import { Component, computed, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { KickoffService } from '@shared/services/kickoff.service';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { KickoffActionItem, KickoffActionItemStatus } from '@shared/models/kickoff.model';
import { AuthService } from '@shared/services/auth';
import { KICKOFF_MESSAGES } from '@shared/constants/kickoff.constants';
import { ButtonComponent } from '@common/components/button/button';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { DialogService } from '@common/services/dialog/dialog.service';
import { ToastService } from '@common/services/toast/toast';
import { AddActionDialogComponent, AddActionDialogResult } from '../../dialogs/add-action-dialog/add-action-dialog.component';

@Component({
  selector: 'app-meeting-record',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, SelectComponent, StatusBadgeComponent, TextareaComponent],
  templateUrl: './meeting-record.component.html',
  styleUrl: './meeting-record.component.scss'
})
export class MeetingRecordComponent {
  private route = inject(ActivatedRoute);
  private kickoffService = inject(KickoffService);
  private dialog = inject(DialogService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);
  private toastService = inject(ToastService);

  readonly messages = KICKOFF_MESSAGES;

  readonly isSignedOff = computed(() => this.engagementService.isSignedOff());
  isProjectManager = computed(() => !this.isSignedOff() && this.authService.hasRole('Project Management'));

  readonly statusOptions: SelectOption[] = [
    { value: 'InProgress', label: 'In Progress' },
    { value: 'Done', label: 'Done' }
  ];

  private readonly statusLabels: Record<KickoffActionItemStatus, string> = {
    Open: 'In Progress',
    InProgress: 'In Progress',
    Done: 'Done'
  };

  statusLabel(status: KickoffActionItemStatus): string {
    return this.statusLabels[status];
  }

  getStatusTone(status: KickoffActionItemStatus): StatusBadgeTone {
    return status === 'Done' ? 'success' : 'neutral';
  }

  sortedActionItems = computed(() =>
    [...this.actionItems()].sort((a, b) => (this.canEditStatus(b) ? 1 : 0) - (this.canEditStatus(a) ? 1 : 0))
  );

  canEditStatus(item: KickoffActionItem): boolean {
    if (this.isSignedOff() || item.status === 'Done') return false;

    const currentUserId = this.authService.currentUser()?.id;
    return currentUserId != null && currentUserId === item.assignedToUserId;
  }

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  outcomes = signal('');
  actionItems = signal<KickoffActionItem[]>([]);
  isSaving = signal(false);

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadRecord(id);
      }
    });
  }

  private loadRecord(id: number): void {
    this.kickoffService.getKickoffRecord(id).subscribe({
      next: (record) => {
        this.outcomes.set(record.outcomes ?? '');
        this.actionItems.set(record.actionItems ?? []);
      },
      error: () => {
        this.outcomes.set('');
        this.actionItems.set([]);
      }
    });
  }

  saveOutcomes(): void {
    const id = this.engagementId();
    if (!id) return;

    this.isSaving.set(true);
    this.kickoffService.saveKickoffRecord(id, this.outcomes()).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.toastService.success(this.messages.OUTCOMES_SAVED_SUCCESS);
      },
      error: () => {
        this.isSaving.set(false);
        this.toastService.error('Failed to save outcomes');
      }
    });
  }

  onStatusChange(item: KickoffActionItem, status: unknown): void {
    const id = this.engagementId();
    if (!id || !item.kickoffActionItemId || !this.canEditStatus(item)) return;

    this.kickoffService.updateActionItemStatus(id, item.kickoffActionItemId, status as KickoffActionItemStatus).subscribe({
      next: (updated) => {
        this.actionItems.update((items) =>
          items.map((existing) => (existing.kickoffActionItemId === updated.kickoffActionItemId ? updated : existing))
        );
        this.toastService.success(this.messages.STATUS_UPDATED_SUCCESS(updated.status));
      },
      error: () => this.toastService.error('Failed to update status')
    });
  }

  openAddActionDialog(): void {
    const id = this.engagementId();
    if (!id || !this.isProjectManager()) return;

    const dialogRef = this.dialog.open<AddActionDialogComponent, { engagementId: number }, AddActionDialogResult>(AddActionDialogComponent, {
      data: { engagementId: id }
    });

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;

      this.kickoffService.createActionItem(id, result).subscribe({
        next: (created) => {
          this.actionItems.update((items) => [...items, created]);
          this.toastService.success(this.messages.ACTION_CREATED_SUCCESS);
        },
        error: () => this.toastService.error('Failed to create action item')
      });
    });
  }
}
