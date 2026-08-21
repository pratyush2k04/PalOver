import { Component, Input, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuditLogService } from './services/audit-log.service';
import { AuditLogDto, EngagementAuditLogDto, AuditCategory, SimpleUserDto } from './models/audit-log.model';
import { AUDIT_LOG_MESSAGES, USER_ROLES } from '@shared/constants/app.constants';
import { AuthService } from '@shared/services/auth';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { PaginatorComponent } from '@common/components/paginator/paginator';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { PageHeaderComponent } from '@common/components/page-header/page-header';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';
import { downloadCsv } from '@shared/utils/csv.util';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent, AlertComponent, EmptyStateComponent, PaginatorComponent, ButtonComponent, InputComponent, PageHeaderComponent, SectionHeaderComponent, SelectComponent],
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

  filterFromDate: string | null = null;
  filterToDate: string | null = null;
  
  filterUserId: number | null = null;
  filterCategory: AuditCategory | null = null;
  
  availableUsers = signal<SimpleUserDto[]>([]);

  userOptions = computed<SelectOption[]>(() => {
    const opts: SelectOption[] = [{ value: null, label: 'All Users' }];
    for (const u of this.availableUsers()) {
      opts.push({ value: u.id, label: u.name });
    }
    return opts;
  });

  categoryOptions: SelectOption[] = [
    { value: null, label: 'All Event Types' },
    { value: 'Document', label: 'Document' },
    { value: 'Checklist', label: 'Checklist' },
    { value: 'Risk', label: 'Risk' },
    { value: 'Access', label: 'Access' },
    { value: 'Handover', label: 'Handover' },
    { value: 'Escalation', label: 'Escalation' },
    { value: 'ResourceAllocation', label: 'Resource Allocation' },
    { value: 'TechnicalReadiness', label: 'Technical Readiness' }
  ];

  page = signal(1);
  readonly pageSize = 20;
  totalCount = signal(0);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  ngOnInit(): void {
    if (this.engagementId != null) {
      this.auditLogService.getEngagementAuditUsers(this.engagementId).subscribe({
        next: (users) => this.availableUsers.set(users)
      });
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

  private readonly actionCategoryMap: Record<string, string> = {
    DocumentVersioned: 'Document',
    DocumentUploaded: 'Document',
    MasterChecklistPublished: 'Checklist',
    UnauthorizedAccess: 'Access',
    AccountLockout: 'Access',
    UserLoggedIn: 'Access',
    UserCreated: 'Access',
    UserUpdated: 'Access',
    UserDeleted: 'Access',
    ResourcingGapFlagRaised: 'Escalation',
    ResourcingGapFlagCleared: 'Escalation',
    ClientContactDetailsChanged: 'Handover',
    ClientPersonaChanged: 'Handover',
    LeadershipAllocated: 'Resource Allocation',
    UserRoleAllocated: 'Resource Allocation',
    PmAndTlAssigned: 'Resource Allocation',
    PmAndTlReassigned: 'Resource Allocation',
    DayOneSummaryGenerated: 'Technical Readiness',
    BrownfieldItemAdded: 'Technical Readiness',
    DiscoveryItemAdded: 'Technical Readiness'
  };

  getEventCategory(action: string): string {
    return this.actionCategoryMap[action] || action;
  }

  exportCsv(): void {
    const rows = this.pagedLogs();
    const header = this.showUserColumn ? ['Date/Time', 'User', 'Event Type', 'Details'] : ['Date/Time', 'Event Type', 'Details'];

    const dataRows = rows.map((log) => {
      const fields = [new Date(log.createdAt).toLocaleString()];
      if (this.showUserColumn) {
        fields.push((log as EngagementAuditLogDto).userName || '');
      }
      fields.push(this.getEventCategory(log.action), log.details);
      return fields;
    });

    downloadCsv(dataRows, header, 'audit-log-export');
  }

  private loadPage(): void {
    this.isLoading.set(true);

    const categories = this.filterCategory ? [this.filterCategory] : undefined;

    const request$ = this.engagementId != null
      ? this.auditLogService.getEngagementAuditLogPage(this.engagementId, this.page(), this.pageSize, this.filterUserId, categories)
      : this.auditLogService.getAuditLogPage(this.page(), this.pageSize, categories);

    request$.subscribe({
      next: (result) => {
        let items = result.items;
        
        if (this.engagementId == null) {
          items = items
            .filter(log => !this.filterFromDate || log.createdAt >= this.filterFromDate)
            .filter(log => !this.filterToDate || log.createdAt <= this.filterToDate);
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
}
