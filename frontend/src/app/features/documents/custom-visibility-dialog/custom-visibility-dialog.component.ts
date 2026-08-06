import { Component, DestroyRef, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DocumentService } from '../../../services/document.service';
import { DocumentItem } from '../../../models/document.model';

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
  readonly availableRoles = [
    { name: 'Leadership', label: 'Leadership' },
    { name: 'Tech Lead', label: 'Tech Lead' },
    { name: 'Pre Sales', label: 'Pre Sales' }
  ];

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
      this.errorMessage.set('Please select at least one role to restrict access.');
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
          const msg = err.error?.message || 'Failed to update custom visibility.';
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
          const msg = err.error?.message || 'Failed to remove visibility override.';
          this.errorMessage.set(msg);
        }
      });
  }

  onCancel(): void {
    this.closed.emit();
  }
}
