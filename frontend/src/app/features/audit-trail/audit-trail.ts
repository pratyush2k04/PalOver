import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditTrailService } from './services/audit-trail.service';
import { AuditLogDto } from './models/audit-log.model';
import { AUDIT_LOG_MESSAGES } from '@shared/constants/app.constants';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { PaginatorComponent } from '@common/components/paginator/paginator';
import { ButtonComponent } from '@common/components/button/button';

interface AuditTrailFilters {
  search: string;
  fromDate: string | null;
  toDate: string | null;
}

function matchesSearch(log: AuditLogDto, term: string): boolean {
  return log.userName.toLowerCase().includes(term)
    || log.action.toLowerCase().includes(term)
    || log.details.toLowerCase().includes(term);
}

function escapeCsvField(value: string): string {
  if (/[",\n]/.test(value)) {
    return `"${value.replace(/"/g, '""')}"`;
  }
  return value;
}

@Component({
  selector: 'app-audit-trail',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent, AlertComponent, EmptyStateComponent, PaginatorComponent, ButtonComponent],
  templateUrl: './audit-trail.html',
  styleUrl: './audit-trail.scss'
})
export class AuditTrailComponent implements OnInit {
  private auditTrailService = inject(AuditTrailService);

  allLogs = signal<AuditLogDto[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  filterSearch = '';
  filterFromDate: string | null = null;
  filterToDate: string | null = null;

  filters = signal<AuditTrailFilters>({ search: '', fromDate: null, toDate: null });
  page = signal(1);
  readonly pageSize = 20;

  filteredLogs = computed(() => {
    const f = this.filters();
    const term = f.search.trim().toLowerCase();
    return this.allLogs()
      .filter((log) => !f.fromDate || log.createdAt >= f.fromDate)
      .filter((log) => !f.toDate || log.createdAt <= f.toDate)
      .filter((log) => !term || matchesSearch(log, term))
      .sort((a, b) => b.createdAt.localeCompare(a.createdAt));
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.filteredLogs().length / this.pageSize)));

  pagedLogs = computed(() => {
    const start = (this.page() - 1) * this.pageSize;
    return this.filteredLogs().slice(start, start + this.pageSize);
  });

  ngOnInit(): void {
    this.loadAllLogs();
  }

  onFilterChange(): void {
    this.filters.set({
      search: this.filterSearch,
      fromDate: this.filterFromDate,
      toDate: this.filterToDate
    });
    this.page.set(1);
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.page()) {
      return;
    }
    this.page.set(page);
  }

  exportCsv(): void {
    const rows = this.filteredLogs();
    const header = ['Date/Time', 'User', 'Action', 'Details'];
    const lines = [header.join(',')];

    for (const log of rows) {
      lines.push([
        escapeCsvField(log.createdAt),
        escapeCsvField(log.userName),
        escapeCsvField(log.action),
        escapeCsvField(log.details)
      ].join(','));
    }

    const csv = lines.join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    const date = new Date().toISOString().slice(0, 10);
    link.href = url;
    link.download = `audit-trail-export-${date}.csv`;
    link.click();
    URL.revokeObjectURL(url);
  }

  private loadAllLogs(cursor: number | null = null, accumulated: AuditLogDto[] = []): void {
    this.isLoading.set(true);
    this.auditTrailService.getAuditLogPage(cursor, 100).subscribe({
      next: (result) => {
        const combined = [...accumulated, ...result.items];
        if (result.hasMore && result.nextCursor != null) {
          this.loadAllLogs(result.nextCursor, combined);
        } else {
          this.allLogs.set(combined);
          this.isLoading.set(false);
        }
      },
      error: () => {
        this.errorMessage.set(AUDIT_LOG_MESSAGES.LOAD_ERROR);
        this.isLoading.set(false);
      }
    });
  }
}
