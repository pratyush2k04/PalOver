import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { LoadingSpinnerComponent } from '../../../shared/loading-spinner/loading-spinner';
import { ChecklistService } from '../checklist.service';
import { ChecklistVersionSummary } from '../checklist.model';
import {
  VersionDetailDialogComponent,
  VersionDetailDialogData,
} from '../dialogs/version-detail/version-detail-dialog';

@Component({
  selector: 'app-version-history',
  standalone: true,
  imports: [DatePipe, MatTableModule, MatButtonModule, MatIconModule, LoadingSpinnerComponent, MatDialogModule],
  templateUrl: './version-history.html',
  styleUrl: './version-history.scss',
})
export class VersionHistoryComponent implements OnInit {
  private checklistService = inject(ChecklistService);
  private dialog = inject(MatDialog);

  readonly loading = this.checklistService.versionsLoading;
  readonly error = this.checklistService.versionsError;
  readonly versions = this.checklistService.versions;

  readonly displayedColumns = ['versionNumber', 'statusName', 'createdAt', 'publishedAt', 'view'];

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
