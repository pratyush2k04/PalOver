import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { SelectOption } from '@common/components/form/select/select';
import { MultiSelectListComponent } from '@common/components/multi-select-list/multi-select-list';
import { DocumentItem, DocumentVisibility, UpdateDocumentVisibilityRequest } from '../../../models/document.model';
import { DOCUMENT_MESSAGES, DOCUMENT_CONFIG } from '@shared/constants/document.constants';
import { EngagementRoleId } from '@shared/constants/users.constants';
import { DocumentService } from '../../../services/document.service';

export interface CustomVisibilityDialogData {
  document: DocumentItem;
}

export type CustomVisibilityDialogResult = true;

@Component({
  selector: 'app-custom-visibility-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, InlineSVGModule, AlertComponent, ButtonComponent, MultiSelectListComponent],
  templateUrl: './custom-visibility-dialog.component.html',
  styleUrl: './custom-visibility-dialog.component.scss'
})
export class CustomVisibilityDialogComponent implements OnInit {
  private dialogRef = inject<DialogRef<CustomVisibilityDialogResult>>(DialogRef);
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  readonly data = inject<CustomVisibilityDialogData>(DIALOG_DATA);

  // Project Management (ID 2) is intentionally excluded — PMs always have access regardless.
  readonly availableRoles = DOCUMENT_CONFIG.AVAILABLE_CUSTOM_VISIBILITY_ROLES;

  readonly roleOptions: SelectOption[] = this.availableRoles.map((role) => ({
    value: role.id,
    label: role.label
  }));

  selectedRoles = signal<number[]>([]);
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);

  ngOnInit(): void {
    const roles = this.data.document.allowedRoles;
    if (roles && roles.length > 0) {
      const nonPmRoles = roles.filter(r => r !== EngagementRoleId.ProjectManager);
      this.selectedRoles.set(nonPmRoles.length > 0 ? nonPmRoles : [EngagementRoleId.Leadership]);
    } else {
      this.selectedRoles.set([EngagementRoleId.Leadership]);
    }
  }

  onRoleSelectionChange(values: unknown[]): void {
    this.selectedRoles.set(values as number[]);
  }

  onSave(): void {
    const roles = this.selectedRoles();
    if (roles.length === 0) {
      this.errorMessage.set(DOCUMENT_MESSAGES.CUSTOM_VISIBILITY_NO_ROLE_SELECTED);
      return;
    }

    this.submit(
      { visibility: 'Custom' as DocumentVisibility, allowedRoles: roles, resetToCategoryDefault: false },
      DOCUMENT_MESSAGES.CUSTOM_VISIBILITY_UPDATE_FAILED,
    );
  }

  onRemoveOverride(): void {
    this.submit({ resetToCategoryDefault: true }, DOCUMENT_MESSAGES.CUSTOM_VISIBILITY_REMOVE_FAILED);
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  private submit(payload: UpdateDocumentVisibilityRequest, failureMessage: string): void {
    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.documentService
      .updateVisibility(this.data.document.id, payload, true)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.errorMessage.set(err?.error?.message || err?.error || failureMessage);
        },
      });
  }
}
