import { Component, ElementRef, HostListener, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { SEARCH_CONFIG, SEARCH_MESSAGES } from '@shared/constants/search.constants';
import { APP_ROUTES, ENGAGEMENT_ROUTES } from '@shared/constants/app.constants';
import {
  GlobalSearchResponse,
  SearchChecklistItemResult,
  SearchDocumentResult,
  SearchEngagementResult,
  SearchPersonResult,
  SearchType
} from '@shared/models/global-search.model';
import { GlobalSearchService } from '@shared/services/global-search.service';

export interface ContentTypeOption {
  type: SearchType;
  label: string;
}

@Component({
  selector: 'app-global-search',
  standalone: true,
  imports: [InlineSVGModule, AlertComponent, EmptyStateComponent, LoadingSpinnerComponent],
  templateUrl: './global-search.html',
  styleUrl: './global-search.scss'
})
export class GlobalSearchComponent {
  private searchService = inject(GlobalSearchService);
  private router = inject(Router);
  private elementRef = inject(ElementRef);

  readonly minQueryLength = SEARCH_CONFIG.MIN_QUERY_LENGTH;
  readonly messages = SEARCH_MESSAGES;

  readonly contentTypes: ContentTypeOption[] = [
    { type: 'engagement', label: 'Engagements' },
    { type: 'document', label: 'Documents' },
    { type: 'person', label: 'People' },
    { type: 'checklist', label: 'Checklist Items' },
  ];

  readonly searchTerm = signal('');
  readonly isOpen = signal(false);
  readonly isLoading = signal(false);
  readonly hasError = signal(false);
  readonly results = signal<GlobalSearchResponse | null>(null);

  // Filter state (persists across search queries in session)
  readonly activeFilters = signal<Set<SearchType>>(new Set());
  readonly cachedFullResults = signal<GlobalSearchResponse | null>(null);

  readonly activeFilterList = computed(() => Array.from(this.activeFilters()));
  readonly hasActiveFilters = computed(() => this.activeFilters().size > 0);

  private debounceHandle: ReturnType<typeof setTimeout> | null = null;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.isOpen() && !this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }

  get noResultsMessage(): string {
    return SEARCH_MESSAGES.NO_RESULTS(this.searchTerm().trim());
  }

  onFocus(): void {
    if (this.searchTerm().trim().length > 0) {
      this.isOpen.set(true);
    }
  }

  onInput(value: string): void {
    this.searchTerm.set(value);
    this.isOpen.set(true);

    if (this.debounceHandle) {
      clearTimeout(this.debounceHandle);
    }

    const term = value.trim();
    if (term.length < this.minQueryLength) {
      this.results.set(null);
      this.cachedFullResults.set(null);
      this.hasError.set(false);
      return;
    }

    const types = this.activeFilters().size > 0 ? Array.from(this.activeFilters()) : undefined;
    this.debounceHandle = setTimeout(() => this.runSearch(term, types), SEARCH_CONFIG.DEBOUNCE_MS);
  }

  isFilterActive(type: SearchType): boolean {
    return this.activeFilters().has(type);
  }

  toggleFilter(type: SearchType): void {
    const current = new Set(this.activeFilters());
    if (current.has(type)) {
      current.delete(type);
    } else {
      current.add(type);
    }
    this.activeFilters.set(current);
    this.applyFilters();
  }

  removeFilter(type: SearchType): void {
    const current = new Set(this.activeFilters());
    current.delete(type);
    this.activeFilters.set(current);
    this.applyFilters();
  }

  clearAllFilters(): void {
    this.activeFilters.set(new Set());
    this.applyFilters();
  }

  getTypeLabel(type: SearchType): string {
    return this.contentTypes.find((c) => c.type === type)?.label ?? type;
  }

  getTypeCount(type: SearchType): number {
    const res = this.results();
    if (!res) return 0;
    switch (type) {
      case 'engagement':
        return res.engagements?.count ?? 0;
      case 'document':
        return res.documents?.count ?? 0;
      case 'person':
        return res.people?.count ?? 0;
      case 'checklist':
        return res.checklistItems?.count ?? 0;
    }
  }

  private applyFilters(): void {
    const term = this.searchTerm().trim();
    if (term.length < this.minQueryLength) {
      return;
    }

    if (this.activeFilters().size === 0) {
      // AC 4: Clearing filter returns to full result set without re-running search
      const cached = this.cachedFullResults();
      if (cached && cached.query === term) {
        this.results.set(cached);
        return;
      }
      this.runSearch(term);
    } else {
      this.runSearch(term, Array.from(this.activeFilters()));
    }
  }

  selectEngagement(item: SearchEngagementResult): void {
    this.close();
    this.router.navigate([APP_ROUTES.ENGAGEMENTS, item.engagementId, ENGAGEMENT_ROUTES.WORKSPACE]);
  }

  selectDocument(item: SearchDocumentResult): void {
    this.close();
    this.router.navigate([APP_ROUTES.ENGAGEMENTS, item.engagementId, ENGAGEMENT_ROUTES.WORKSPACE, ENGAGEMENT_ROUTES.DOCUMENTS]);
  }

  selectPerson(item: SearchPersonResult): void {
    this.close();
    this.router.navigate([APP_ROUTES.ENGAGEMENTS, item.engagementId, ENGAGEMENT_ROUTES.WORKSPACE, ENGAGEMENT_ROUTES.PEOPLE]);
  }

  selectChecklistItem(item: SearchChecklistItemResult): void {
    this.close();
    this.router.navigate([APP_ROUTES.ENGAGEMENTS, item.engagementId, ENGAGEMENT_ROUTES.WORKSPACE, ENGAGEMENT_ROUTES.CHECKLIST]);
  }

  private runSearch(term: string, types?: SearchType[]): void {
    this.isLoading.set(true);
    this.hasError.set(false);

    const isUnfiltered = !types || types.length === 0;

    this.searchService.search(term, types).subscribe({
      next: (response) => {
        this.results.set(response);
        if (isUnfiltered) {
          this.cachedFullResults.set(response);
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.results.set(null);
        this.hasError.set(true);
        this.isLoading.set(false);
      }
    });
  }

  private close(): void {
    this.isOpen.set(false);
  }
}
