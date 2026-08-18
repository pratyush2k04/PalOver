import { Component, Input, OnChanges, SimpleChanges, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AlertComponent } from '@common/components/alert/alert';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { EngagementService, EngagementOpenItemsSummary } from '../../../../services/engagement.service';
import { OPEN_ITEMS_ERROR_MESSAGES } from '@shared/constants/engagements.constants';

@Component({
  selector: 'app-open-items-by-area',
  standalone: true,
  imports: [CommonModule, RouterLink, AlertComponent, LoadingSpinnerComponent],
  templateUrl: './open-items-by-area.component.html',
  styleUrls: ['./open-items-by-area.component.scss']
})
export class OpenItemsByAreaComponent implements OnChanges {
  @Input() engagementId?: number;

  private engagementService = inject(EngagementService);

  summary = signal<EngagementOpenItemsSummary | null>(null);
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['engagementId'] && this.engagementId) {
      this.loadSummary(this.engagementId);
    }
  }

  loadSummary(id: number): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.engagementService.getOpenItemsSummary(id).subscribe({
      next: (data: EngagementOpenItemsSummary) => {
        this.summary.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.summary.set(null);
        const errorMessage = err.error?.message || (
          err.status === 404
            ? OPEN_ITEMS_ERROR_MESSAGES.ENGAGEMENT_NOT_FOUND
            : OPEN_ITEMS_ERROR_MESSAGES.LOAD_FAILED
        );
        this.error.set(errorMessage);
        this.isLoading.set(false);
      }
    });
  }
}
