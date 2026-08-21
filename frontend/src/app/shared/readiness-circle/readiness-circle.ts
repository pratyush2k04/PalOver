import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

export type ReadinessCircleSize = 'sm' | 'md' | 'lg';
export type ReadinessCircleTone = 'danger' | 'warning' | 'success';

@Component({
  selector: 'app-readiness-circle',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './readiness-circle.html',
  styleUrl: './readiness-circle.scss'
})
export class ReadinessCircleComponent {
  readonly score = input<number | null>(0);
  readonly size = input<ReadinessCircleSize>('md');

  readonly scorePercent = computed(() => {
    const s = this.score();
    return s !== null && s !== undefined ? Math.round(s) : 0;
  });

  readonly tone = computed<ReadinessCircleTone>(() => {
    const s = this.scorePercent();
    if (s < 10) return 'danger';
    if (s > 90) return 'success';
    return 'warning';
  });

  readonly strokeDashoffset = computed(() => {
    const c = 251.327; // 2 * Math.PI * 40
    return c - (c * this.scorePercent()) / 100;
  });
}
