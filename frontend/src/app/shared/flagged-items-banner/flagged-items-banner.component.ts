import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface FlaggedItem {
  label: string;
  count: number;
}

@Component({
  selector: 'app-flagged-items-banner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './flagged-items-banner.component.html',
  styleUrl: './flagged-items-banner.component.scss'
})
export class FlaggedItemsBannerComponent {
  items = input.required<FlaggedItem[]>();
  allClearMessage = input<string>('All kickoff-readiness checks passed.');

  flaggedItems = computed(() => this.items().filter(i => i.count > 0));
}
