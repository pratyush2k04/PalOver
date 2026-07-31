import { Component, OnInit, computed, inject } from '@angular/core';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { LoadingSpinnerComponent } from '../../../shared/loading-spinner/loading-spinner';
import { ChecklistService } from '../checklist.service';
import { ChecklistItem, ChecklistItemOrder } from '../checklist.model';
import { ItemFormDialogComponent, ItemFormDialogData, ItemFormDialogResult } from '../dialogs/item-form/item-form-dialog';
import { ConfirmDialogComponent, ConfirmDialogData } from '../dialogs/confirm/confirm-dialog';

@Component({
  selector: 'app-current-draft',
  standalone: true,
  imports: [
    DragDropModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    LoadingSpinnerComponent,
  ],
  templateUrl: './current-draft.html',
  styleUrl: './current-draft.scss',
})
export class CurrentDraftComponent implements OnInit {
  private checklistService = inject(ChecklistService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  readonly loading = this.checklistService.draftLoading;
  readonly error = this.checklistService.draftError;
  readonly notFound = this.checklistService.draftNotFound;
  readonly draft = this.checklistService.currentDraft;
  readonly categories = this.checklistService.categories;

  readonly activeItems = computed(() =>
    (this.draft()?.items ?? [])
      .filter((item) => !item.isRetired)
      .sort((a, b) => a.displayOrder - b.displayOrder)
  );

  readonly retiredItems = computed(() =>
    (this.draft()?.items ?? []).filter((item) => item.isRetired)
  );

  ngOnInit(): void {
    this.checklistService.getCurrentDraft();
    this.checklistService.loadCategories();
  }

  private notify(message: string): void {
    this.snackBar.open(message, 'Dismiss', { duration: 4000 });
  }

  private reload(): void {
    this.checklistService.getCurrentDraft();
  }

  onAddItem(): void {
    const nextDisplayOrder = this.activeItems().length + 1;

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

      this.checklistService
        .createItem({
          title: result.title,
          description: result.description,
          categoryId: result.categoryId,
          isMandatory: result.isMandatory,
          displayOrder: nextDisplayOrder,
        })
        .subscribe({
          next: () => {
            this.notify('Item added.');
            this.reload();
          },
          error: () => this.notify('Unable to add item. Please try again.'),
        });
    });
  }

  onEditItem(item: ChecklistItem): void {
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

      this.checklistService
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

  onRetireItem(item: ChecklistItem): void {
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

      this.checklistService.retireItem(item.id).subscribe({
        next: () => {
          this.notify('Item retired.');
          this.reload();
        },
        error: () => this.notify('Unable to retire item. Please try again.'),
      });
    });
  }

  onDrop(event: CdkDragDrop<ChecklistItem[]>): void {
    const items = [...this.activeItems()];
    moveItemInArray(items, event.previousIndex, event.currentIndex);

    const orders: ChecklistItemOrder[] = items.map((item, index) => ({
      itemId: item.id,
      displayOrder: index + 1,
    }));

    this.checklistService.reorderItems({ items: orders }).subscribe({
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

      this.checklistService.publishVersion(draft.id).subscribe({
        next: () => {
          this.notify('Version published.');
          this.reload();
        },
        error: () => this.notify('Unable to publish version. Please try again.'),
      });
    });
  }
}
