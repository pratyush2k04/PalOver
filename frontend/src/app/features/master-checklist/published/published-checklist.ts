import { Component, OnInit, computed, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, UpperCasePipe } from '@angular/common';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { BadgeComponent } from '@common/components/badge/badge';
import { ButtonComponent } from '@common/components/button/button';
import { MasterChecklistService } from '../services/master-checklist.service';
import { MasterChecklistItem } from '../models/master-checklist.model';

@Component({
  selector: 'app-published-checklist',
  standalone: true,
  imports: [
    FormsModule,
    LoadingSpinnerComponent,
    StatusBadgeComponent,
    AlertComponent,
    EmptyStateComponent,
    BadgeComponent,
    ButtonComponent,
    UpperCasePipe,
    DatePipe
  ],
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

  readonly categories = computed(() => {
    const items = this.version()?.items ?? [];
    const names = new Set(items.map((item) => item.categoryName));
    return Array.from(names).sort();
  });

  readonly groupedCategories = computed(() => {
    const items = this.version()?.items ?? [];
    const selected = this.selectedCategory();

    const filteredItems = selected === 'all'
      ? items
      : items.filter(item => item.categoryName === selected);

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
