import { Component, DestroyRef, OnInit, inject, input, output, signal } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DocumentService } from '../../../services/document.service';
import { DocumentItem } from '../../../models/document.model';
import { DOCUMENT_MESSAGES, DOCUMENT_CONFIG } from '../../../shared/constants/document.constants';

@Component({
  selector: 'app-custom-visibility-dialog',
  standalone: true,
  imports: [FormsModule, InlineSVGModule],
  templateUrl: './custom-visibility-dialog.component.html',
  styleUrl: './custom-visibility-dialog.component.scss'
})
export class CustomVisibilityDialogComponent implements OnInit {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);

  document = input.required<DocumentItem>();
  closed = output<void>();
  updated = output<DocumentItem>();

  // Project Management (ID 2) is intentionally excluded — PMs always have access regardless.
  readonly availableRoles = DOCUMENT_CONFIG.AVAILABLE_CUSTOM_VISIBILITY_ROLES;

  selectedRoles = signal<number[]>([]);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    const roles = this.document().allowedRoles;
    if (roles && roles.length > 0) {
      const nonPmRoles = roles.filter(r => r !== DOCUMENT_CONFIG.ROLE_IDS.PROJECT_MANAGEMENT);
      this.selectedRoles.set(nonPmRoles.length > 0 ? nonPmRoles : [DOCUMENT_CONFIG.ROLE_IDS.LEADERSHIP]);
    } else {
      this.selectedRoles.set([DOCUMENT_CONFIG.ROLE_IDS.LEADERSHIP]);
    }
  }

  isRoleSelected(roleId: number): boolean {
    return this.selectedRoles().includes(roleId);
  }

  toggleRole(roleId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.selectedRoles.update((roles) => {
      if (checked) {
        return roles.includes(roleId) ? roles : [...roles, roleId];
      } else {
        return roles.filter((r) => r !== roleId);
      }
    });
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
      .updateVisibility(this.document().id, {
        visibility: 'Custom',
        allowedRoles: roles,
        resetToCategoryDefault: false
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (doc) => {
          this.isSubmitting.set(false);
          this.updated.emit(doc);
          this.closed.emit();
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
      .updateVisibility(this.document().id, {
        resetToCategoryDefault: true
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (doc) => {
          this.isSubmitting.set(false);
          this.updated.emit(doc);
          this.closed.emit();
        },
        error: (err) => {
          this.isSubmitting.set(false);
          const msg = err.error?.message || DOCUMENT_MESSAGES.CUSTOM_VISIBILITY_REMOVE_FAILED;
          this.errorMessage.set(msg);
        }
      });
  }

  onCancel(): void {
    this.closed.emit();
  }
}
