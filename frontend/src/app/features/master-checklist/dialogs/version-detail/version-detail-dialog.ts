import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DialogRef } from '../../../../common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '../../../../common/services/dialog/dialog.service';
import { LoadingSpinnerComponent } from '../../../../common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '../../../../common/components/status-badge/status-badge';
import { ButtonComponent } from '../../../../common/components/button/button';
import { MasterChecklistService } from '../../services/master-checklist.service';
import { MasterChecklistItem, MasterChecklistVersion } from '../../models/master-checklist.model';

export interface VersionDetailDialogData {
  versionId: number;
}

interface CategoryGroup {
  categoryName: string;
  items: MasterChecklistItem[];
}

@Component({
  selector: 'app-version-detail-dialog',
  standalone: true,
  imports: [LoadingSpinnerComponent, StatusBadgeComponent, ButtonComponent],
  templateUrl: './version-detail-dialog.html',
  styleUrl: './version-detail-dialog.scss',
})
export class VersionDetailDialogComponent implements OnInit {
  private masterChecklistService = inject(MasterChecklistService);
  private dialogRef = inject(DialogRef);
  readonly data = inject<VersionDetailDialogData>(DIALOG_DATA);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly version = signal<MasterChecklistVersion | null>(null);

  readonly categoryGroups = computed<CategoryGroup[]>(() => {
    const items = this.version()?.items ?? [];
    const groups = new Map<string, MasterChecklistItem[]>();

    for (const item of items) {
      const group = groups.get(item.categoryName) ?? [];
      group.push(item);
      groups.set(item.categoryName, group);
    }

    return Array.from(groups.entries())
      .sort(([a], [b]) => a.localeCompare(b))
      .map(([categoryName, groupItems]) => ({
        categoryName,
        items: groupItems.sort((a, b) => a.displayOrder - b.displayOrder),
      }));
  });

  ngOnInit(): void {
    this.masterChecklistService.getVersionById(this.data.versionId).subscribe({
      next: (version) => {
        this.version.set(version);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load version details. Please try again.');
        this.loading.set(false);
      },
    });
  }

  onClose(): void {
    this.dialogRef.close();
  }
}
