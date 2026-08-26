import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { DayOneReadinessSummaryDto } from '@features/engagements/models/technical-readiness/technical-summary.dtos';

@Component({
  selector: 'app-summary-print-pillars',
  standalone: true,
  imports: [CommonModule, StatusBadgeComponent],
  templateUrl: './summary-print-pillars.component.html',
  styleUrl: './summary-print-pillars.component.scss'
})
export class SummaryPrintPillarsComponent {
  summary = input.required<DayOneReadinessSummaryDto>();

  accessStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (s.isAccessReady) return 'success';
    if (s.blockedAccessCount > 0) return 'danger';
    return 'warning';
  });

  toolingStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (s.isToolingReady) return 'success';
    return 'warning';
  });

  architectureStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (s.isSizingRiskRaised) return 'danger';
    return 'success';
  });

  brownfieldStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (!s.isBrownfieldApplicable) return 'neutral';
    if (s.isBrownfieldReady) return 'success';
    return 'danger';
  });

  dependencySpikesStatusTone = computed<StatusBadgeTone>(() => {
    const s = this.summary();
    if (s.totalDependencySpikes === 0) return 'neutral';
    if (s.failedDependencySpikes > 0) return 'danger';
    if (s.inProgressDependencySpikes > 0) return 'warning';
    return 'success';
  });
}
