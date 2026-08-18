import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuditLogService } from './services/audit-log.service';
import { AuditLogDto, AuditLogFilters } from './models/audit-log.model';
import { AUDIT_LOG_MESSAGES, USER_ROLES } from '@shared/constants/app.constants';
import { AuthService } from 'src/app/services/auth';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { PaginatorComponent } from '@common/components/paginator/paginator';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { downloadCsv } from '@shared/utils/csv.util';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent, AlertComponent, EmptyStateComponent, PaginatorComponent, ButtonComponent, InputComponent],
  templateUrl: './audit-log.html',
  styleUrl: './audit-log.scss'
})
export class AuditLogComponent implements OnInit {
  private auditLogService = inject(AuditLogService);
  private authService = inject(AuthService);

  readonly showUserColumn = this.authService.hasRole(USER_ROLES.LEADERSHIP) || this.authService.isAdmin();

  allLogs = signal<AuditLogDto[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  filterSearch = '';
  filterFromDate: string | null = null;
  filterToDate: string | null = null;

  filters = signal<AuditLogFilters>({ search: '', fromDate: null, toDate: null });
  page = signal(1);
  readonly pageSize = 20;

  filteredLogs = computed(() => {
    const f = this.filters();
    const term = f.search.trim().toLowerCase();
    return this.allLogs()
      .filter((log) => !f.fromDate || log.createdAt >= f.fromDate)
      .filter((log) => !f.toDate || log.createdAt <= f.toDate)
      .filter((log) => !term || this.matchesSearch(log, term))
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
    const header = this.showUserColumn ? ['Date/Time', 'User', 'Action', 'Details'] : ['Date/Time', 'Action', 'Details'];

    const dataRows = rows.map((log) => {
      const fields = [new Date(log.createdAt).toLocaleString()];
      if (this.showUserColumn) {
        fields.push(log.userName);
      }
      fields.push(log.action, log.details);
      return fields;
    });

    downloadCsv(dataRows, header, 'audit-log-export');
  }

  private matchesSearch(log: AuditLogDto, term: string): boolean {
    return log.userName.toLowerCase().includes(term)
      || log.action.toLowerCase().includes(term)
      || log.details.toLowerCase().includes(term);
  }

  private loadAllLogs(cursor: number | null = null, accumulated: AuditLogDto[] = []): void {
    this.isLoading.set(true);
    this.auditLogService.getAuditLogPage(cursor, 100).subscribe({
      next: (result) => {
        const combined = [...accumulated, ...result.items];
        if (result.hasMore && result.nextCursor != null) {
          this.loadAllLogs(result.nextCursor, combined);
        } else {
          this.allLogs.set(combined);
          this.isLoading.set(false);
        }
      },
      error: (error: HttpErrorResponse) => {
        const message = error.status === 401 ? AUDIT_LOG_MESSAGES.UNAUTHORIZED_ERROR : AUDIT_LOG_MESSAGES.LOAD_ERROR;
        this.errorMessage.set(message);
        this.isLoading.set(false);
      }
    });
  }
}
