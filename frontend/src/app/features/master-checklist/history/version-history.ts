import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DialogService } from '../../../common/services/dialog/dialog.service';
import { LoadingSpinnerComponent } from '../../../common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent, StatusBadgeTone } from '../../../common/components/status-badge/status-badge';
import { ButtonComponent } from '../../../common/components/button/button';
import { MasterChecklistService } from '../services/master-checklist.service';
import { MasterChecklistVersionSummary } from '../models/master-checklist.model';
import {
  VersionDetailDialogComponent,
  VersionDetailDialogData,
} from '../dialogs/version-detail/version-detail-dialog';

@Component({
  selector: 'app-version-history',
  standalone: true,
  imports: [DatePipe, LoadingSpinnerComponent, StatusBadgeComponent, ButtonComponent],
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

  statusTone(statusName: string): StatusBadgeTone {
    if (statusName === 'Draft') return 'warning';
    if (statusName === 'Published') return 'success';
    return 'neutral';
  }

  onView(version: MasterChecklistVersionSummary): void {
    this.dialog.open<VersionDetailDialogComponent, VersionDetailDialogData>(VersionDetailDialogComponent, {
      data: { versionId: version.id },
    });
  }
}
