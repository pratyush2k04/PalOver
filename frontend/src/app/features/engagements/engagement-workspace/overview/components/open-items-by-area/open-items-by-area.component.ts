import { Component, Input, OnChanges, SimpleChanges, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EngagementService, EngagementOpenItemsSummary } from '../../../../services/engagement.service';

@Component({
  selector: 'app-open-items-by-area',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './open-items-by-area.component.html',
  styleUrls: ['./open-items-by-area.component.scss']
})
export class OpenItemsByAreaComponent implements OnChanges {
  @Input() engagementId?: number;

  private engagementService = inject(EngagementService);

  summary = signal<EngagementOpenItemsSummary | null>(null);
  isLoading = signal<boolean>(false);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['engagementId'] && this.engagementId) {
      this.loadSummary(this.engagementId);
    }
  }

  loadSummary(id: number): void {
    this.isLoading.set(true);
    this.engagementService.getOpenItemsSummary(id).subscribe({
      next: (data: EngagementOpenItemsSummary) => {
        this.summary.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.summary.set(null);
        this.isLoading.set(false);
      }
    });
  }
}
