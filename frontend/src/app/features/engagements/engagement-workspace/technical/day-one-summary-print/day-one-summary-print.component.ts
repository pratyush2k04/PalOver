import { Component, inject, signal, computed, effect, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { TechnicalReadinessService } from '@features/engagements/services/technical-readiness.service';
import { DayOneReadinessSummaryDto } from '@features/engagements/models/technical-readiness/technical-summary.dtos';
import { SummaryPrintHeaderComponent } from './components/summary-print-header/summary-print-header.component';
import { SummaryPrintHeroComponent } from './components/summary-print-hero/summary-print-hero.component';
import { SummaryPrintPillarsComponent } from './components/summary-print-pillars/summary-print-pillars.component';
import { SummaryPrintBlockersComponent } from './components/summary-print-blockers/summary-print-blockers.component';

@Component({
  selector: 'app-day-one-summary-print',
  standalone: true,
  imports: [
    CommonModule,
    InlineSVGModule,
    ButtonComponent,
    SummaryPrintHeaderComponent,
    SummaryPrintHeroComponent,
    SummaryPrintPillarsComponent,
    SummaryPrintBlockersComponent
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
