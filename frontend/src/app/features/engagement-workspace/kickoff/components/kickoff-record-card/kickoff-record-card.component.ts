import { Component, Input, OnChanges, SimpleChanges, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { KickoffService } from '../../../../../services/kickoff.service';
import { ToastService } from '../../../../../services/toast';
import { AuthService } from '../../../../../services/auth';
import { KickoffRecord, KickoffActionItem, KickoffActionItemStatus } from '../../../../../models/kickoff.model';
import { KICKOFF_MESSAGES } from '../../../../../constants/kickoff.constants';

@Component({
  selector: 'app-kickoff-record-card',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './kickoff-record-card.component.html',
  styleUrls: ['./kickoff-record-card.component.scss']
})
export class KickoffRecordCardComponent implements OnChanges {
  @Input() engagementId?: number;

  private kickoffService = inject(KickoffService);
  private toastService = inject(ToastService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  readonly isProjectManager = computed(() => {
    const user = this.authService.currentUser();
    const role = user?.role || '';
    return role === 'Project Management' || this.authService.hasRole('Project Management');
  });

  record = signal<KickoffRecord | null>(null);
  outcomesText = signal<string>('');
  actionItems = signal<KickoffActionItem[]>([]);
  
  isLoading = signal<boolean>(false);
  isSavingNotes = signal<boolean>(false);
  
  showAddActionForm = signal<boolean>(false);
  isCreatingAction = signal<boolean>(false);

  newActionForm = this.fb.group({
    title: ['', [Validators.required]],
    ownerName: ['', [Validators.required]],
    dueDate: ['', [Validators.required]]
  });

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['engagementId'] && this.engagementId) {
      this.loadKickoffRecord(this.engagementId);
    }
  }

  loadKickoffRecord(id: number): void {
    this.isLoading.set(true);
    this.kickoffService.getKickoffRecord(id).subscribe({
      next: (data) => {
        this.record.set(data);
        this.outcomesText.set(data?.outcomes || '');
        this.actionItems.set(data?.actionItems || []);
        this.isLoading.set(false);
      },
      error: () => {
        this.record.set(null);
        this.actionItems.set([]);
        this.isLoading.set(false);
      }
    });
  }

  onOutcomesChange(value: string): void {
    if (this.isProjectManager()) {
      this.outcomesText.set(value);
    }
  }

  onOutcomesBlur(): void {
    this.saveOutcomes();
  }

  saveOutcomes(): void {
    if (!this.engagementId || !this.isProjectManager()) return;
    this.isSavingNotes.set(true);
    this.kickoffService.saveKickoffRecord(this.engagementId, this.outcomesText()).subscribe({
      next: () => {
        this.isSavingNotes.set(false);
        this.toastService.success(KICKOFF_MESSAGES.OUTCOMES_SAVED_SUCCESS);
      },
      error: () => {
        this.isSavingNotes.set(false);
      }
    });
  }

  onStatusChange(item: KickoffActionItem, event: Event): void {
    if (!this.isProjectManager()) return;
    const target = event.target as HTMLSelectElement;
    if (!target || !this.engagementId || !item.kickoffActionItemId) return;
    const newStatus = target.value as KickoffActionItemStatus;
    
    this.actionItems.update(items =>
      items.map(i => i.kickoffActionItemId === item.kickoffActionItemId ? { ...i, status: newStatus } : i)
    );

    this.kickoffService.updateActionItemStatus(this.engagementId, item.kickoffActionItemId, newStatus).subscribe({
      next: () => {
        this.toastService.success(KICKOFF_MESSAGES.STATUS_UPDATED_SUCCESS(newStatus));
      },
      error: () => {
        this.loadKickoffRecord(this.engagementId!);
      }
    });
  }

  toggleAddActionForm(): void {
    if (!this.isProjectManager()) return;
    this.showAddActionForm.set(!this.showAddActionForm());
    if (!this.showAddActionForm()) {
      this.newActionForm.reset();
    }
  }

  submitNewAction(): void {
    if (!this.isProjectManager()) return;

    if (this.newActionForm.invalid || !this.engagementId) {
      this.newActionForm.markAllAsTouched();
      this.toastService.error(KICKOFF_MESSAGES.PLEASE_FIX_HIGHLIGHTED_FIELDS);
      return;
    }

    this.isCreatingAction.set(true);
    const formValue = this.newActionForm.value;

    const dto: Partial<KickoffActionItem> = {
      engagementId: this.engagementId,
      title: formValue.title?.trim() || '',
      ownerName: formValue.ownerName?.trim() || '',
      dueDate: formValue.dueDate || new Date().toISOString().split('T')[0],
      status: 'Open'
    };

    this.kickoffService.createActionItem(this.engagementId, dto).subscribe({
      next: (created) => {
        this.actionItems.update(items => [...items, created]);
        this.isCreatingAction.set(false);
        this.toastService.success(KICKOFF_MESSAGES.ACTION_CREATED_SUCCESS);
        this.toggleAddActionForm();
      },
      error: () => {
        this.isCreatingAction.set(false);
      }
    });
  }

  deleteActionItem(item: KickoffActionItem): void {
    if (!this.isProjectManager() || !this.engagementId || !item.kickoffActionItemId) return;
    if (!confirm(KICKOFF_MESSAGES.DELETE_ACTION_CONFIRM(item.title))) return;

    this.actionItems.update(items => items.filter(i => i.kickoffActionItemId !== item.kickoffActionItemId));

    this.kickoffService.deleteActionItem(this.engagementId, item.kickoffActionItemId).subscribe({
      next: () => {
        this.toastService.success(KICKOFF_MESSAGES.ACTION_DELETED_SUCCESS);
      },
      error: () => {
        this.loadKickoffRecord(this.engagementId!);
      }
    });
  }

  formatDueDate(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return dateStr;
    return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
