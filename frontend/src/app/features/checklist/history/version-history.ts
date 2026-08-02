import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DialogService } from '../../../shared/dialog/dialog.service';
import { LoadingSpinnerComponent } from '../../../shared/loading-spinner/loading-spinner';
import { ChecklistService } from '../../../services/checklist.service';
import { ChecklistVersionSummary } from '../../../models/checklist.model';
import {
  VersionDetailDialogComponent,
  VersionDetailDialogData,
} from '../dialogs/version-detail/version-detail-dialog';

@Component({
  selector: 'app-version-history',
  standalone: true,
  imports: [DatePipe, LoadingSpinnerComponent],
  templateUrl: './version-history.html',
  styleUrl: './version-history.scss',
})
export class VersionHistoryComponent implements OnInit {
  private checklistService = inject(ChecklistService);
  private dialog = inject(DialogService);

  readonly loading = this.checklistService.versionsLoading;
  readonly error = this.checklistService.versionsError;
  readonly versions = this.checklistService.versions;

  ngOnInit(): void {
    this.checklistService.getVersionHistory();
  }

  publishedAt(version: ChecklistVersionSummary): string | null {
    return version.statusName === 'Published' ? version.updatedAt : null;
  }

  onView(version: ChecklistVersionSummary): void {
    this.dialog.open<VersionDetailDialogComponent, VersionDetailDialogData>(VersionDetailDialogComponent, {
      data: { versionId: version.id },
    });
  }
}
