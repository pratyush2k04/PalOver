import { Component, inject, signal, computed, effect, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { TechnicalReadinessService } from '@features/engagements/services/technical-readiness.service';
import { DayOneReadinessSummaryDto } from '@features/engagements/models/technical-readiness/technical-summary.dtos';

@Component({
  selector: 'app-day-one-summary-print',
  standalone: true,
  imports: [
    CommonModule,
    InlineSVGModule,
    StatusBadgeComponent,
    ButtonComponent
  ],
  templateUrl: './day-one-summary-print.component.html',
  styleUrl: './day-one-summary-print.component.scss'
})
export class DayOneSummaryPrintComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private technicalService = inject(TechnicalReadinessService);
  private titleService = inject(Title);

  summary = signal<DayOneReadinessSummaryDto | null>(null);
  isLoading = signal<boolean>(true);
  error = signal<string | null>(null);

  isAbleToStart = computed(() => this.summary()?.isAbleToStartDayOne ?? false);

  statusTone = computed<StatusBadgeTone>(() =>
    this.isAbleToStart() ? 'success' : 'danger'
  );

  accessStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (!s) return 'neutral';
    if (s.isAccessReady) return 'success';
    if (s.blockedAccessCount > 0) return 'danger';
    return 'warning';
  });

  toolingStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (!s) return 'neutral';
    if (s.isToolingReady) return 'success';
    return 'warning';
  });

  architectureStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (!s) return 'neutral';
    if (s.isSizingRiskRaised) return 'danger';
    return 'success';
  });

  brownfieldStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (!s || !s.isBrownfieldApplicable) return 'neutral';
    if (s.isBrownfieldReady) return 'success';
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

    this.loadSummary(engagementId);
  }

  loadSummary(engagementId: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.technicalService.getTechnicalSummary(engagementId).subscribe({
      next: (data) => {
        this.summary.set(data);
        this.isLoading.set(false);
        this.titleService.setTitle(`${data.engagementName} - Day-1 Technical Readiness Summary`);
        
        // Auto-invoke print dialog after brief rendering delay
        setTimeout(() => {
          window.print();
        }, 600);
      },
      error: () => {
        this.error.set('Failed to load Day-1 Technical Readiness Summary.');
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
