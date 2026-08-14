import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { HandoverQueryService } from '../../../../../services/handover-query.service';
import { CommitmentDto } from '../../../../../models/handover.model';
import { HandoverQueryDto, HandoverQueryFieldSection } from '../../../../../models/handover-query.model';

export interface RaiseHandoverQueryDialogData {
  engagementId: number;
  commitments: CommitmentDto[];
}

const FIELD_SECTION_OPTIONS: SelectOption[] = [
  { value: HandoverQueryFieldSection.ClientBackground, label: 'Client Background' },
  { value: HandoverQueryFieldSection.BusinessProblem, label: 'Business Problem' },
  { value: HandoverQueryFieldSection.ClientSentiment, label: 'Client Sentiment' },
  { value: HandoverQueryFieldSection.DealOrigin, label: 'Deal Origin' },
  { value: HandoverQueryFieldSection.Commitment, label: 'Commitment' },
];

@Component({
  selector: 'app-raise-handover-query-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, AlertComponent, SelectComponent, TextareaComponent],
  templateUrl: './raise-handover-query-dialog.component.html',
  styleUrl: './raise-handover-query-dialog.component.scss',
})
export class RaiseHandoverQueryDialogComponent {
  private dialogRef = inject<DialogRef<HandoverQueryDto>>(DialogRef);
  private handoverQueryService = inject(HandoverQueryService);
  readonly data = inject<RaiseHandoverQueryDialogData>(DIALOG_DATA);

  readonly fieldSectionOptions = FIELD_SECTION_OPTIONS;
  readonly commitmentOptions = computed<SelectOption[]>(() =>
    this.data.commitments.map((c) => ({ value: c.commitmentId, label: c.title }))
  );

  fieldSection = signal<HandoverQueryFieldSection | null>(null);
  referenceId = signal<number | null>(null);
  question = signal('');

  isSubmitting = signal(false);
  errorMessage = signal('');

  isCommitmentSection = computed(() => this.fieldSection() === HandoverQueryFieldSection.Commitment);

  canSubmit = computed(() => {
    if (!this.fieldSection() || !this.question().trim()) return false;
    if (this.isCommitmentSection() && !this.referenceId()) return false;
    return true;
  });

  onFieldSectionChange(value: HandoverQueryFieldSection): void {
    this.fieldSection.set(value);
    if (value !== HandoverQueryFieldSection.Commitment) {
      this.referenceId.set(null);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.canSubmit()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    this.handoverQueryService
      .raiseQuery(this.data.engagementId, {
        fieldSection: this.fieldSection()!,
        referenceId: this.referenceId() ?? undefined,
        question: this.question().trim(),
      })
      .subscribe({
        next: (created) => {
          this.isSubmitting.set(false);
          this.dialogRef.close(created);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          const msg = (err as { error?: { message?: string } })?.error?.message;
          this.errorMessage.set(msg || 'Failed to raise query.');
        },
      });
  }
}
