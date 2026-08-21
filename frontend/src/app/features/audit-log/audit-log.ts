import { Component, Input, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuditLogService } from './services/audit-log.service';
import { AuditLogDto, AuditLogFilters, EngagementAuditLogDto, AuditCategory } from './models/audit-log.model';
import { AUDIT_LOG_MESSAGES, USER_ROLES } from '@shared/constants/app.constants';
import { AuthService } from '@shared/services/auth';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { PaginatorComponent } from '@common/components/paginator/paginator';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent } from '@common/components/form/select/select';
import { downloadCsv } from '@shared/utils/csv.util';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent, AlertComponent, EmptyStateComponent, PaginatorComponent, ButtonComponent, InputComponent, SelectComponent],
  templateUrl: './audit-log.html',
  styleUrl: './audit-log.scss'
})
export class AuditLogComponent implements OnInit {
  private auditLogService = inject(AuditLogService);
  private authService = inject(AuthService);

  @Input() engagementId: number | null = null;

  get showUserColumn(): boolean {
    return this.engagementId != null
      || this.authService.hasRole(USER_ROLES.LEADERSHIP)
      || this.authService.isAdmin();
  }

  get emptyStateMessage(): string {
    return this.engagementId != null
      ? 'No activity in the last 30 days.'
      : 'No audit log entries found.';
  }

  pagedLogs = signal<(AuditLogDto | EngagementAuditLogDto)[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  filterSearch = '';
  filterFromDate: string | null = null;
  filterToDate: string | null = null;
  
  filterUserId: number | null = null;
  filterCategories: AuditCategory[] = [];

  page = signal(1);
  readonly pageSize = 20;
  totalCount = signal(0);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  // For global in-memory filtering fallback if engagementId == null
  // We keep this because global audits still fetch all via API offset pagination?
  // Wait, the plan was to remove cursor traces for EVERYTHING. So global audits should also just fetch the page.
  
  ngOnInit(): void {
    if (this.engagementId != null) {
      // Don't auto-set dates for backend filtered view unless needed by business logic
      // Removing the 30-day default for engagement as backend doesn't filter by date yet
    }
    this.loadPage();
  }

  onFilterChange(): void {
    this.page.set(1);
    this.loadPage();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.page()) {
      return;
    }
    this.page.set(page);
    this.loadPage();
  }

  exportCsv(): void {
    const rows = this.pagedLogs();
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

  private loadPage(): void {
    this.isLoading.set(true);

    const request$ = this.engagementId != null
      ? this.auditLogService.getEngagementAuditLogPage(this.engagementId, this.page(), this.pageSize, this.filterUserId, this.filterCategories)
      : this.auditLogService.getAuditLogPage(this.page(), this.pageSize);

    request$.subscribe({
      next: (result) => {
        let items = result.items;
        
        // If global audits, still apply in-memory text/date filtering to the *current page* 
        // (Since global backend API doesn't support text/date filters yet)
        if (this.engagementId == null) {
          const term = this.filterSearch.trim().toLowerCase();
          items = items
            .filter(log => !this.filterFromDate || log.createdAt >= this.filterFromDate)
            .filter(log => !this.filterToDate || log.createdAt <= this.filterToDate)
            .filter(log => !term || this.matchesSearch(log, term));
        }
        
        this.pagedLogs.set(items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        const message = error.status === 401 ? AUDIT_LOG_MESSAGES.UNAUTHORIZED_ERROR : AUDIT_LOG_MESSAGES.LOAD_ERROR;
        this.errorMessage.set(message);
        this.isLoading.set(false);
      }
    });
  }

  private matchesSearch(log: AuditLogDto | EngagementAuditLogDto, term: string): boolean {
    return log.userName.toLowerCase().includes(term)
      || log.action.toLowerCase().includes(term)
      || log.details.toLowerCase().includes(term);
  }
}
