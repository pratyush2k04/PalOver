import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ReadinessOnePagerDto, ReadinessOnePagerService } from '@shared/services/readiness-one-pager.service';
import { PORTFOLIO_STATUS } from '@shared/constants/portfolio.constants';
import { ENGAGEMENT_ROUTES } from '@shared/constants/routes.constants';

@Component({
  selector: 'app-readiness-one-pager-dialog',
  standalone: true,
  imports: [CommonModule, DatePipe, ButtonComponent, AlertComponent, StatusBadgeComponent, LoadingSpinnerComponent, EmptyStateComponent],
  templateUrl: './readiness-one-pager-dialog.component.html',
  styleUrl: './readiness-one-pager-dialog.component.scss'
})
export class ReadinessOnePagerDialogComponent implements OnInit {
  private dialogRef = inject(DialogRef<void>);
  public data = inject(DIALOG_DATA) as { engagementId: number };
  private onePagerService = inject(ReadinessOnePagerService);

  readonly report = signal<ReadinessOnePagerDto | null>(null);
  readonly isLoading = signal<boolean>(true);
  readonly error = signal<string | null>(null);

  readonly statusTone = computed<StatusBadgeTone>(() => {
    const report = this.report();
    if (!report) return 'neutral';
    if (report.status === PORTFOLIO_STATUS.KICKOFF_READY) return 'success';
    if (report.status === PORTFOLIO_STATUS.AT_RISK) return 'danger';
    return 'warning';
  });

  readonly scoreTone = computed<StatusBadgeTone>(() => {
    const score = this.report()?.score ?? 0;
    if (score === 100) return 'success';
    if (score >= 60) return 'info';
    if (score >= 40) return 'warning';
    return 'danger';
  });

  ngOnInit(): void {
    this.onePagerService.getOnePager(this.data.engagementId).subscribe({
      next: (report) => {
        this.report.set(report);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load the kickoff-readiness one-pager. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  exportAsPdf(): void {
    window.open(`/engagements/${this.data.engagementId}/${ENGAGEMENT_ROUTES.READINESS_ONE_PAGER_PRINT}`, '_blank');
  }
}
