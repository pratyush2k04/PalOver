import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DialogService } from '../../../shared/dialog/dialog.service';
import { LoadingSpinnerComponent } from '../../../shared/loading-spinner/loading-spinner';
import { MasterChecklistService } from '../../../services/master-checklist.service';
import { MasterChecklistVersionSummary } from '../../../models/checklist.model';
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
  private masterChecklistService = inject(MasterChecklistService);
  private dialog = inject(DialogService);

  readonly loading = this.masterChecklistService.versionsLoading;
  readonly error = this.masterChecklistService.versionsError;
  readonly versions = this.masterChecklistService.versions;

  ngOnInit(): void {
    this.masterChecklistService.getVersionHistory();
  }

  publishedAt(version: MasterChecklistVersionSummary): string | null {
    return version.statusName === 'Published' ? version.updatedAt : null;
  }

  onView(version: MasterChecklistVersionSummary): void {
    this.dialog.open<VersionDetailDialogComponent, VersionDetailDialogData>(VersionDetailDialogComponent, {
      data: { versionId: version.id },
    });
  }
}
