import { Component, OnInit, computed, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoadingSpinnerComponent } from '../../../shared/loading-spinner/loading-spinner';
import { ChecklistService } from '../../../services/checklist.service';
import { ChecklistItem } from '../../../models/checklist.model';

type SortColumn = 'title' | 'categoryName' | 'isMandatory' | 'isRetired';
type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-published-checklist',
  standalone: true,
  imports: [FormsModule, LoadingSpinnerComponent],
  templateUrl: './published-checklist.html',
  styleUrl: './published-checklist.scss',
})
export class PublishedChecklistComponent implements OnInit {
  private checklistService = inject(ChecklistService);

  readonly loading = this.checklistService.publishedLoading;
  readonly error = this.checklistService.publishedError;
  readonly notFound = this.checklistService.publishedNotFound;
  readonly version = this.checklistService.publishedChecklist;

  readonly selectedCategory = signal<string>('all');
  readonly expandedRowId = signal<number | null>(null);

  readonly sortColumn = signal<SortColumn>('title');
  readonly sortDirection = signal<SortDirection>('asc');

  readonly pageIndex = signal(0);
  readonly pageSize = signal(10);
  readonly pageSizeOptions = [10, 25, 50];

  readonly categories = computed(() => {
    const items = this.version()?.items ?? [];
    const names = new Set(items.map((item) => item.categoryName));
    return Array.from(names).sort();
  });

  readonly filteredItems = computed(() => {
    const items = this.version()?.items ?? [];
    const category = this.selectedCategory();

    const filtered = items.filter((item) => category === 'all' || item.categoryName === category);

    const column = this.sortColumn();
    const direction = this.sortDirection() === 'asc' ? 1 : -1;
    return [...filtered].sort((a, b) => {
      const aValue = a[column];
      const bValue = b[column];
      if (aValue === bValue) return 0;
      return aValue > bValue ? direction : -direction;
    });
  });

  readonly pagedItems = computed(() => {
    const start = this.pageIndex() * this.pageSize();
    return this.filteredItems().slice(start, start + this.pageSize());
  });

  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.filteredItems().length / this.pageSize())));

  ngOnInit(): void {
    this.checklistService.getPublished();
  }

  applyFilters(): void {
    this.pageIndex.set(0);
  }

  sortBy(column: SortColumn): void {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
  }

  toggleRow(item: ChecklistItem): void {
    this.expandedRowId.set(this.expandedRowId() === item.id ? null : item.id);
  }

  isExpanded(item: ChecklistItem): boolean {
    return this.expandedRowId() === item.id;
  }

  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.pageIndex.set(0);
  }

  goToFirstPage(): void {
    this.pageIndex.set(0);
  }

  goToPreviousPage(): void {
    this.pageIndex.set(Math.max(0, this.pageIndex() - 1));
  }

  goToNextPage(): void {
    this.pageIndex.set(Math.min(this.totalPages() - 1, this.pageIndex() + 1));
  }

  goToLastPage(): void {
    this.pageIndex.set(this.totalPages() - 1);
  }
}
