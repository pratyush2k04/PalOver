import { Component, OnInit, computed, effect, inject, signal, ViewChild, AfterViewInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { LoadingSpinnerComponent } from '../../../shared/loading-spinner/loading-spinner';
import { ChecklistService } from '../checklist.service';
import { ChecklistItem } from '../checklist.model';

@Component({
  selector: 'app-published-checklist',
  standalone: true,
  imports: [
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    LoadingSpinnerComponent,
  ],
  templateUrl: './published-checklist.html',
  styleUrl: './published-checklist.scss',
})
export class PublishedChecklistComponent implements OnInit, AfterViewInit {
  private checklistService = inject(ChecklistService);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  readonly loading = this.checklistService.publishedLoading;
  readonly error = this.checklistService.publishedError;
  readonly notFound = this.checklistService.publishedNotFound;
  readonly version = this.checklistService.publishedChecklist;

  readonly displayedColumns = ['title', 'categoryName', 'isMandatory', 'isRetired', 'expand'];
  readonly dataSource = new MatTableDataSource<ChecklistItem>([]);

  readonly searchTerm = signal('');
  readonly selectedCategory = signal<string>('all');
  readonly expandedRowId = signal<number | null>(null);

  readonly categories = computed(() => {
    const items = this.version()?.items ?? [];
    const names = new Set(items.map((item) => item.categoryName));
    return Array.from(names).sort();
  });

  constructor() {
    effect(() => {
      this.dataSource.data = this.version()?.items ?? [];
    });
  }

  ngOnInit(): void {
    this.checklistService.getPublished();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (item, filter) => {
      const { search, category } = JSON.parse(filter) as { search: string; category: string };
      const matchesCategory = category === 'all' || item.categoryName === category;
      const matchesSearch =
        !search ||
        item.title.toLowerCase().includes(search) ||
        (item.description ?? '').toLowerCase().includes(search);
      return matchesCategory && matchesSearch;
    };
  }

  applyFilters(): void {
    this.dataSource.filter = JSON.stringify({
      search: this.searchTerm().trim().toLowerCase(),
      category: this.selectedCategory(),
    });
  }

  toggleRow(item: ChecklistItem): void {
    this.expandedRowId.set(this.expandedRowId() === item.id ? null : item.id);
  }

  isExpanded(item: ChecklistItem): boolean {
    return this.expandedRowId() === item.id;
  }
}
