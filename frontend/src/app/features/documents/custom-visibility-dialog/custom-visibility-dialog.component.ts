import { Component, DestroyRef, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DocumentService } from '../../../services/document.service';
import { DocumentItem } from '../../../models/document.model';
import { DOCUMENT_MESSAGES, DOCUMENT_CONFIG } from '../../../shared/constants/document.constants';

@Component({
  selector: 'app-custom-visibility-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './custom-visibility-dialog.component.html',
  styleUrl: './custom-visibility-dialog.component.scss'
})
export class CustomVisibilityDialogComponent {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);

  @Input({ required: true }) document!: DocumentItem;
  @Output() closed = new EventEmitter<void>();
  @Output() updated = new EventEmitter<DocumentItem>();

  // Project Management is intentionally excluded — PMs always have access regardless.
  readonly availableRoles = DOCUMENT_CONFIG.AVAILABLE_CUSTOM_VISIBILITY_ROLES;

  selectedRoles = signal<string[]>(
    this.document?.allowedRoles?.length
      ? [...this.document.allowedRoles].filter(r => r !== 'Project Management')
      : ['Leadership']   // sensible non-PM default
  );
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  isRoleSelected(roleName: string): boolean {
    return this.selectedRoles().includes(roleName);
  }

  toggleRole(roleName: string, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.selectedRoles.update((roles) => {
      if (checked) {
        return roles.includes(roleName) ? roles : [...roles, roleName];
      } else {
        return roles.filter((r) => r !== roleName);
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
      .updateVisibility(this.document.id, {
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
      .updateVisibility(this.document.id, {
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
