import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { SelectOption } from '@common/components/form/select/select';
import { MultiSelectListComponent } from '@common/components/multi-select-list/multi-select-list';
import { DocumentService } from '../../../services/document.service';
import { DocumentItem } from '../../../models/document.model';
import { DOCUMENT_MESSAGES, DOCUMENT_CONFIG } from '@shared/constants/document.constants';

export interface CustomVisibilityDialogData {
  document: DocumentItem;
}

@Component({
  selector: 'app-custom-visibility-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, InlineSVGModule, AlertComponent, ButtonComponent, MultiSelectListComponent],
  templateUrl: './custom-visibility-dialog.component.html',
  styleUrl: './custom-visibility-dialog.component.scss'
})
export class CustomVisibilityDialogComponent implements OnInit {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  private dialogRef = inject<DialogRef<DocumentItem>>(DialogRef);
  readonly data = inject<CustomVisibilityDialogData>(DIALOG_DATA);

  // Project Management (ID 2) is intentionally excluded — PMs always have access regardless.
  readonly availableRoles = DOCUMENT_CONFIG.AVAILABLE_CUSTOM_VISIBILITY_ROLES;

  readonly roleOptions: SelectOption[] = this.availableRoles.map((role) => ({
    value: role.id,
    label: role.label
  }));

  selectedRoles = signal<number[]>([]);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    const roles = this.data.document.allowedRoles;
    if (roles && roles.length > 0) {
      const nonPmRoles = roles.filter(r => r !== DOCUMENT_CONFIG.ROLE_IDS.PROJECT_MANAGEMENT);
      this.selectedRoles.set(nonPmRoles.length > 0 ? nonPmRoles : [DOCUMENT_CONFIG.ROLE_IDS.LEADERSHIP]);
    } else {
      this.selectedRoles.set([DOCUMENT_CONFIG.ROLE_IDS.LEADERSHIP]);
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

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.documentService
      .updateVisibility(this.data.document.id, {
        visibility: 'Custom',
        allowedRoles: roles,
        resetToCategoryDefault: false
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (doc) => {
          this.isSubmitting.set(false);
          this.dialogRef.close(doc);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          const msg = err.error?.message || DOCUMENT_MESSAGES.CUSTOM_VISIBILITY_UPDATE_FAILED;
          this.errorMessage.set(msg);
        }
      });
  }

  onRemoveOverride(): void {
    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.documentService
      .updateVisibility(this.data.document.id, {
        resetToCategoryDefault: true
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (doc) => {
          this.isSubmitting.set(false);
          this.dialogRef.close(doc);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          const msg = err.error?.message || DOCUMENT_MESSAGES.CUSTOM_VISIBILITY_REMOVE_FAILED;
          this.errorMessage.set(msg);
        }
      });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
