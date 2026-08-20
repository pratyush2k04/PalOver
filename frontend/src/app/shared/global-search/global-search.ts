import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
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
  SearchPersonResult
} from '@shared/models/global-search.model';
import { GlobalSearchService } from '@shared/services/global-search.service';

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

  readonly searchTerm = signal('');
  readonly isOpen = signal(false);
  readonly isLoading = signal(false);
  readonly hasError = signal(false);
  readonly results = signal<GlobalSearchResponse | null>(null);

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
      this.hasError.set(false);
      return;
    }

    this.debounceHandle = setTimeout(() => this.runSearch(term), SEARCH_CONFIG.DEBOUNCE_MS);
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

  private runSearch(term: string): void {
    this.isLoading.set(true);
    this.hasError.set(false);

    this.searchService.search(term).subscribe({
      next: (response) => {
        this.results.set(response);
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
