import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { EngagementSortBy, EngagementService, PmEngagementListItem } from '../../services/engagement';

interface MyEngagementRow extends PmEngagementListItem {
  displayReadinessScore: number;
  isOverdue: boolean;
}

type EngagementTab = 'active' | 'past';
type SortOption = 'kickoff' | 'readiness' | 'client';

// TODO: replace with real readiness data once checklist-based scoring is implemented
const SAMPLE_READINESS_SCORES: Record<number, number> = {
  1: 69,
  4: 74,
  5: 100
};

@Component({
  selector: 'app-my-engagements',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-engagements.html',
  styleUrl: './my-engagements.scss'
})
export class MyEngagementsComponent implements OnInit {
  private engagementService = inject(EngagementService);
  private router = inject(Router);

  activeTab = signal<EngagementTab>('active');
  sortBy = signal<SortOption>('kickoff');
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  engagements = signal<MyEngagementRow[]>([]);

  // Past Engagements has no backing data yet - static placeholder until billing sign-off exists.
  readonly pastEngagements: MyEngagementRow[] = [
    {
      engagementId: 9001,
      clientName: 'Aurora Retail Co.',
      engagementTypeName: 'Greenfield',
      commercialModelName: 'Fixed',
      kickoffDate: '2026-05-12',
      daysToKickoff: -81,
      readinessScore: null,
      displayReadinessScore: 100,
      isOverdue: false
    }
  ];

  activeCount = computed(() => this.engagements().length);
  pastCount = computed(() => this.pastEngagements.length);

  ngOnInit(): void {
    this.loadEngagements();
  }

  private loadEngagements(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.engagementService.getMyEngagements(this.sortBy() as EngagementSortBy).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        const rows = response.engagements.map((engagement) => ({
          ...engagement,
          displayReadinessScore: SAMPLE_READINESS_SCORES[engagement.engagementId] ?? 0,
          isOverdue: engagement.daysToKickoff < 0
        }));

        // Readiness scores are sample/client-side data, so the backend can't sort by them - sort here instead.
        if (this.sortBy() === 'readiness') {
          rows.sort((a, b) => b.displayReadinessScore - a.displayReadinessScore);
        }

        this.engagements.set(rows);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Failed to load your engagements. Please try again.');
      }
    });
  }

  selectTab(tab: EngagementTab): void {
    this.activeTab.set(tab);
  }

  onSortChange(sortBy: SortOption): void {
    this.sortBy.set(sortBy);
    this.loadEngagements();
  }

  openEngagement(engagementId: number): void {
    this.router.navigate(['/engagements', engagementId]);
  }

  formatKickoffDate(kickoffDate: string): string {
    const date = new Date(kickoffDate);
    return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
  }

  readinessColor(score: number): string {
    if (score >= 90) return '#22c55e';
    if (score >= 60) return '#f5a623';
    return '#dc2626';
  }

  ringOffset(score: number): number {
    const circumference = 2 * Math.PI * 20;
    return circumference - (score / 100) * circumference;
  }
}
