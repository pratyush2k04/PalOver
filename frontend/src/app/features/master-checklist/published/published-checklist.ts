import { Component, OnInit, computed, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UpperCasePipe } from '@angular/common';
import { LoadingSpinnerComponent } from '../../../shared/loading-spinner/loading-spinner';
import { MasterChecklistService } from '../services/master-checklist.service';
import { MasterChecklistItem } from '../models/master-checklist.model';

type SortColumn = 'title' | 'categoryName' | 'isMandatory' | 'isRetired';
type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-published-checklist',
  standalone: true,
  imports: [FormsModule, LoadingSpinnerComponent, UpperCasePipe],
  templateUrl: './published-checklist.html',
  styleUrl: './published-checklist.scss',
})
export class PublishedChecklistComponent implements OnInit {
  private masterChecklistService = inject(MasterChecklistService);

  readonly loading = this.masterChecklistService.publishedLoading;
  readonly error = this.masterChecklistService.publishedError;
  readonly notFound = this.masterChecklistService.publishedNotFound;
  readonly version = this.masterChecklistService.publishedChecklist;

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

  readonly groupedCategories = computed(() => {
    const items = this.version()?.items ?? [];
    const selected = this.selectedCategory();
    
    // First, filter by the selected category if applicable
    const filteredItems = selected === 'all' 
      ? items 
      : items.filter(item => item.categoryName === selected);

    // Then, group the remaining items by category
    const groupMap = new Map<number, { categoryId: number; categoryName: string; items: MasterChecklistItem[] }>();

    for (const item of filteredItems) {
      if (!groupMap.has(item.categoryId)) {
        groupMap.set(item.categoryId, {
          categoryId: item.categoryId,
          categoryName: item.categoryName,
          items: []
        });
      }
      groupMap.get(item.categoryId)!.items.push(item);
    }

    // Return as array sorted by categoryName
    return Array.from(groupMap.values()).sort((a, b) => a.categoryName.localeCompare(b.categoryName));
  });

  ngOnInit(): void {
    this.masterChecklistService.getPublished();
  }

  toggleRow(item: MasterChecklistItem): void {
    this.expandedRowId.set(this.expandedRowId() === item.id ? null : item.id);
  }

  isExpanded(item: MasterChecklistItem): boolean {
    return this.expandedRowId() === item.id;
  }
}
