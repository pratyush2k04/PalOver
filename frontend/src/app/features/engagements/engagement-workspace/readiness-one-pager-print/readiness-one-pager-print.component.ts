import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { ButtonComponent } from '@common/components/button/button';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ReadinessOnePagerDto, ReadinessOnePagerService } from '@shared/services/readiness-one-pager.service';
import { PORTFOLIO_STATUS } from '@shared/constants/portfolio.constants';

@Component({
  selector: 'app-readiness-one-pager-print',
  standalone: true,
  imports: [CommonModule, DatePipe, ButtonComponent, LoadingSpinnerComponent, StatusBadgeComponent],
  templateUrl: './readiness-one-pager-print.component.html',
  styleUrl: './readiness-one-pager-print.component.scss'
})
export class ReadinessOnePagerPrintComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private onePagerService = inject(ReadinessOnePagerService);
  private titleService = inject(Title);

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
    const idParam = this.route.snapshot.paramMap.get('engagementId') ?? this.route.snapshot.paramMap.get('id');
    const engagementId = idParam ? +idParam : null;

    if (!engagementId || isNaN(engagementId)) {
      this.error.set('Invalid Engagement ID');
      this.isLoading.set(false);
      return;
    }

    this.loadReport(engagementId);
  }

  loadReport(engagementId: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.onePagerService.getOnePager(engagementId).subscribe({
      next: (data) => {
        this.report.set(data);
        this.isLoading.set(false);
        this.titleService.setTitle(`${data.engagementName} - Kickoff Readiness One-Pager`);
      },
      error: () => {
        this.error.set('Failed to load the kickoff-readiness one-pager.');
        this.isLoading.set(false);
      }
    });
  }

  onPrint(): void {
    window.print();
  }

  onClose(): void {
    window.close();
  }
}
