import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DialogService } from '../../../shared/dialog/dialog.service';
import { ToastService } from '../../../shared/toast/toast.service';
import { LoadingSpinnerComponent } from '../../../shared/loading-spinner/loading-spinner';
import { MasterChecklistService } from '../../../services/master-checklist.service';
import { MasterChecklistItem, MasterChecklistItemOrder } from '../../../models/checklist.model';
import { ItemFormDialogComponent, ItemFormDialogData, ItemFormDialogResult } from '../dialogs/item-form/item-form-dialog';
import { ConfirmDialogComponent, ConfirmDialogData } from '../dialogs/confirm/confirm-dialog';

@Component({
  selector: 'app-current-draft',
  standalone: true,
  imports: [LoadingSpinnerComponent],
  templateUrl: './current-draft.html',
  styleUrl: './current-draft.scss',
})
export class CurrentDraftComponent implements OnInit {
  private masterChecklistService = inject(MasterChecklistService);
  private dialog = inject(DialogService);
  private toast = inject(ToastService);

  readonly loading = this.masterChecklistService.draftLoading;
  readonly error = this.masterChecklistService.draftError;
  readonly notFound = this.masterChecklistService.draftNotFound;
  readonly draft = this.masterChecklistService.currentDraft;
  readonly categories = this.masterChecklistService.categories;

  readonly activeItems = computed(() =>
    (this.draft()?.items ?? [])
      .filter((item) => !item.isRetired)
      .sort((a, b) => a.displayOrder - b.displayOrder)
  );

  readonly retiredItems = computed(() =>
    (this.draft()?.items ?? []).filter((item) => item.isRetired)
  );

  readonly draggedItemId = signal<number | null>(null);
  readonly dragOverItemId = signal<number | null>(null);

  ngOnInit(): void {
    this.masterChecklistService.getCurrentDraft();
    this.masterChecklistService.loadCategories();
  }

  private notify(message: string): void {
    this.toast.open(message);
  }

  private reload(): void {
    this.masterChecklistService.getCurrentDraft();
  }

  onAddItem(): void {
    const nextDisplayOrder = 1;

    const dialogRef = this.dialog.open<ItemFormDialogComponent, ItemFormDialogData, ItemFormDialogResult>(
      ItemFormDialogComponent,
      {
        data: { mode: 'create', item: null, categories: this.categories(), nextDisplayOrder },
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }

      this.masterChecklistService
        .createItem({
          title: result.title,
          description: result.description,
          categoryId: result.categoryId,
          isMandatory: result.isMandatory,
          displayOrder: nextDisplayOrder,
        })
        .subscribe({
          next: (createdItem) => this.promoteNewItemToTop(createdItem),
          error: () => this.notify('Unable to add item. Please try again.'),
        });
    });
  }

  private promoteNewItemToTop(createdItem: MasterChecklistItem): void {
    const existingItems = this.activeItems().filter((item) => item.id !== createdItem.id);

    const orders: MasterChecklistItemOrder[] = [
      { itemId: createdItem.id, displayOrder: 1 },
      ...existingItems.map((item, index) => ({
        itemId: item.id,
        displayOrder: index + 2,
      })),
      ...this.retiredItems().map((item, index) => ({
        itemId: item.id,
        displayOrder: existingItems.length + index + 2,
      })),
    ];

    this.masterChecklistService.reorderItems({ items: orders }).subscribe({
      next: () => {
        this.notify('Item added.');
        this.reload();
      },
      error: () => {
        this.notify('Item added, but reordering to top failed.');
        this.reload();
      },
    });
  }

  onEditItem(item: MasterChecklistItem): void {
    const dialogRef = this.dialog.open<ItemFormDialogComponent, ItemFormDialogData, ItemFormDialogResult>(
      ItemFormDialogComponent,
      {
        data: { mode: 'edit', item, categories: this.categories(), nextDisplayOrder: item.displayOrder },
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }

      this.masterChecklistService
        .updateItem(item.id, {
          title: result.title,
          description: result.description,
          categoryId: result.categoryId,
          isMandatory: result.isMandatory,
        })
        .subscribe({
          next: () => {
            this.notify('Item updated.');
            this.reload();
          },
          error: () => this.notify('Unable to update item. Please try again.'),
        });
    });
  }

  onRetireItem(item: MasterChecklistItem): void {
    const dialogRef = this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
      data: {
        title: 'Retire this item?',
        message: `"${item.title}" will be marked as retired and removed from the active list.`,
        confirmLabel: 'Retire',
        isDestructive: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.masterChecklistService.retireItem(item.id).subscribe({
        next: () => {
          this.notify('Item retired.');
          this.reload();
        },
        error: () => this.notify('Unable to retire item. Please try again.'),
      });
    });
  }

  onDragStart(item: MasterChecklistItem): void {
    this.draggedItemId.set(item.id);
  }

  onDragEnter(item: MasterChecklistItem): void {
    if (this.draggedItemId() !== null && this.draggedItemId() !== item.id) {
      this.dragOverItemId.set(item.id);
    }
  }

  onDragEnd(): void {
    this.draggedItemId.set(null);
    this.dragOverItemId.set(null);
  }

  onDrop(targetItem: MasterChecklistItem): void {
    const draggedId = this.draggedItemId();
    this.draggedItemId.set(null);
    this.dragOverItemId.set(null);

    if (draggedId === null || draggedId === targetItem.id) {
      return;
    }

    const items = [...this.activeItems()];
    const fromIndex = items.findIndex((item) => item.id === draggedId);
    const toIndex = items.findIndex((item) => item.id === targetItem.id);
    if (fromIndex === -1 || toIndex === -1) {
      return;
    }

    const [moved] = items.splice(fromIndex, 1);
    items.splice(toIndex, 0, moved);

    const orders: MasterChecklistItemOrder[] = [
      ...items.map((item, index) => ({
        itemId: item.id,
        displayOrder: index + 1,
      })),
      ...this.retiredItems().map((item, index) => ({
        itemId: item.id,
        displayOrder: items.length + index + 1,
      })),
    ];

    this.masterChecklistService.reorderItems({ items: orders }).subscribe({
      next: () => {
        this.notify('Order updated.');
        this.reload();
      },
      error: () => this.notify('Unable to reorder items. Please try again.'),
    });
  }

  onPublish(): void {
    const draft = this.draft();
    if (!draft) {
      return;
    }

    const dialogRef = this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
      data: {
        title: 'Publish this version?',
        message: `Version ${draft.versionNumber} with ${this.activeItems().length} active item(s) will be published and become visible to everyone. A new draft version will be created automatically.`,
        confirmLabel: 'Publish',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.masterChecklistService.publishVersion(draft.id).subscribe({
        next: () => {
          this.notify('Version published.');
          this.reload();
        },
        error: () => this.notify('Unable to publish version. Please try again.'),
      });
    });
  }
}
