import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { NotificationService } from '@shared/services/notification.service';
import { Notification, NotificationPriority } from '@shared/models/notification.model';
import { EngagementService } from '../engagements/services/engagement.service';
import { AuthService } from '@shared/services/auth';
import { ToastService } from '@common/services/toast/toast';
import { NOTIFICATION_MESSAGES } from '@shared/constants/app.constants';

// Reusable Common Components
import { PageHeaderComponent } from '@common/components/page-header/page-header';
import { BadgeComponent } from '@common/components/badge/badge';
import { TabsComponent, TabItem } from '@common/components/tabs/tabs';
import { ButtonComponent } from '@common/components/button/button';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { CheckboxComponent } from '@common/components/form/checkbox/checkbox';
import { PaginatorComponent } from '@common/components/paginator/paginator';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import {
  StatusBadgeComponent,
  StatusBadgeTone,
} from '@common/components/status-badge/status-badge';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { TooltipDirective } from '@common/directives/tooltip.directive';

@Component({
  selector: 'app-notification-fullpage',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InlineSVGModule,
    DatePipe,
    PageHeaderComponent,
    BadgeComponent,
    TabsComponent,
    ButtonComponent,
    SelectComponent,
    CheckboxComponent,
    PaginatorComponent,
    EmptyStateComponent,
    StatusBadgeComponent,
    ConfirmDialogComponent,
    TooltipDirective,
  ],
  templateUrl: './notification-fullpage.component.html',
  styleUrl: './notification-fullpage.component.scss',
})
export class NotificationFullpageComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private engagementService = inject(EngagementService);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  // State Signals
  readonly notifications = signal<Notification[]>([]);
  readonly totalCount = signal<number>(0);
  readonly page = signal<number>(1);
  readonly pageSize = signal<number>(10);
  readonly selectedStatusFilter = signal<string>('all');
  readonly selectedPriorityFilter = signal<unknown>('all');
  readonly selectedEngagementFilter = signal<number | undefined>(undefined);
  readonly engagementFilterOptions = signal<SelectOption[]>([
    { value: 'all', label: 'All Engagements' },
  ]);
  readonly selectedIds = signal<Set<number>>(new Set());
  readonly showDeleteConfirm = signal<boolean>(false);
  readonly isDeleting = signal<boolean>(false);

  readonly NotificationPriority = NotificationPriority;

  readonly filterTabs: TabItem[] = [
    { id: 'all', label: 'All' },
    { id: 'unread', label: 'Unread' },
    { id: 'read', label: 'Read' },
  ];

  readonly priorityOptions: SelectOption[] = [
    { value: 'all', label: 'All Priorities' },
    { value: NotificationPriority.High, label: 'High' },
    { value: NotificationPriority.Medium, label: 'Medium' },
    { value: NotificationPriority.Low, label: 'Low' },
  ];

  // Computed signals
  readonly isAdmin = computed<boolean>(() => this.authService.isAdmin());

  readonly totalPages = computed<number>(() => {
    const total = this.totalCount();
    const size = this.pageSize();
    return total > 0 ? Math.ceil(total / size) : 1;
  });

  readonly filteredNotifications = computed<Notification[]>(() => this.notifications());

  readonly unreadCountInCurrentView = computed<number>(() => {
    return this.notifications().filter((n) => !n.readAt).length;
  });

  readonly selectedCount = computed<number>(() => this.selectedIds().size);

  readonly allSelected = computed<boolean>(() => {
    const current = this.filteredNotifications();
    if (current.length === 0) return false;
    const selected = this.selectedIds();
    return current.every((n) => selected.has(n.id));
  });

  ngOnInit(): void {
    this.loadEngagementOptions();
    this.loadNotifications();
  }

  loadEngagementOptions(): void {
    this.engagementService.getMyEngagements().subscribe({
      next: (res) => {
        const options: SelectOption[] = [
          { value: 'all', label: 'All Engagements' },
          ...res.engagements.map((e) => ({
            value: e.id,
            label: e.engagementName
              ? `${e.engagementName}${e.clientName ? ` (${e.clientName})` : ''}`
              : e.clientName
                ? `${e.clientName} (Engagement #${e.id})`
                : `Engagement #${e.id}`,
          })),
        ];
        this.engagementFilterOptions.set(options);
      },
      error: () => {
        this.toastService.error(NOTIFICATION_MESSAGES.LOAD_ERROR);
      },
    });
  }

  loadNotifications(): void {
    const status = this.selectedStatusFilter();
    const isRead = status === 'read' ? true : status === 'unread' ? false : undefined;

    const priority = this.selectedPriorityFilter();
    const priorityFilter =
      priority === 'all' ? undefined : (Number(priority) as NotificationPriority);

    this.notificationService
      .getNotifications(
        this.selectedEngagementFilter(),
        this.page(),
        this.pageSize(),
        isRead,
        priorityFilter
      )
      .subscribe({
        next: (result) => {
          this.notifications.set(result.items);
          this.totalCount.set(result.totalCount);

          const validIds = new Set(result.items.map((i) => i.id));
          const currentSelections = new Set(
            [...this.selectedIds()].filter((id) => validIds.has(id)),
          );
          this.selectedIds.set(currentSelections);
        },
        error: () => {
          this.toastService.error(NOTIFICATION_MESSAGES.LOAD_ERROR);
        },
      });
  }

  onPageChange(newPage: number): void {
    this.page.set(newPage);
    this.loadNotifications();
  }

  onPageSizeChange(newSize: number): void {
    this.pageSize.set(newSize);
    this.page.set(1);
    this.loadNotifications();
  }

  onStatusFilterChange(status: string): void {
    this.selectedStatusFilter.set(status);
    this.page.set(1);
    this.loadNotifications();
  }

  onPriorityFilterChange(value: unknown): void {
    this.selectedPriorityFilter.set(value);
    this.page.set(1);
    this.loadNotifications();
  }

  onEngagementFilterChange(value: unknown): void {
    if (value === 'all' || value === undefined || value === null) {
      this.selectedEngagementFilter.set(undefined);
    } else {
      this.selectedEngagementFilter.set(Number(value));
    }
    this.page.set(1);
    this.loadNotifications();
  }

  onMarkAsRead(notification: Notification, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    if (notification.readAt) return;

    this.notificationService.markAsRead(notification.id).subscribe({
      next: () => {
        const now = new Date().toISOString();
        this.notifications.update((items) =>
          items.map((n) => (n.id === notification.id ? { ...n, readAt: now, status: 'Read' } : n)),
        );
        this.toastService.success(NOTIFICATION_MESSAGES.MARK_READ_SUCCESS);
      },
      error: () => {
        this.toastService.error(NOTIFICATION_MESSAGES.MARK_READ_ERROR);
      },
    });
  }

  onMarkAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe({
      next: () => {
        const now = new Date().toISOString();
        this.notifications.update((items) =>
          items.map((n) => ({ ...n, readAt: now, status: 'Read' })),
        );
        this.toastService.success(NOTIFICATION_MESSAGES.MARK_ALL_READ_SUCCESS);
      },
      error: () => {
        this.toastService.error(NOTIFICATION_MESSAGES.MARK_ALL_READ_ERROR);
      },
    });
  }

  onNotificationClick(notification: Notification): void {
    if (!notification.readAt) {
      this.onMarkAsRead(notification);
    }

    if (notification.link) {
      let targetRoute = notification.link;
      if (targetRoute.startsWith('/')) {
        this.router.navigateByUrl(targetRoute);
      } else {
        this.router.navigate([targetRoute]);
      }
    }
  }

  toggleSelectAll(checked: boolean): void {
    if (!checked) {
      this.selectedIds.set(new Set());
    } else {
      const allIds = new Set(this.filteredNotifications().map((n) => n.id));
      this.selectedIds.set(allIds);
    }
  }

  toggleSelect(id: number, checked: boolean, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    const set = new Set(this.selectedIds());
    if (checked) {
      set.add(id);
    } else {
      set.delete(id);
    }
    this.selectedIds.set(set);
  }

  promptBulkDelete(): void {
    if (this.selectedIds().size === 0) return;
    this.showDeleteConfirm.set(true);
  }

  cancelBulkDelete(): void {
    this.showDeleteConfirm.set(false);
  }

  onConfirmBulkDelete(): void {
    const idsToDelete = Array.from(this.selectedIds());
    if (idsToDelete.length === 0) return;

    this.isDeleting.set(true);
    this.notificationService.bulkDelete(idsToDelete).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.showDeleteConfirm.set(false);
        this.selectedIds.set(new Set());
        this.toastService.success(NOTIFICATION_MESSAGES.DELETE_SUCCESS);
        this.loadNotifications();
      },
      error: () => {
        this.isDeleting.set(false);
        this.toastService.error(NOTIFICATION_MESSAGES.DELETE_ERROR);
      },
    });
  }

  getPriorityTone(priority: NotificationPriority): StatusBadgeTone {
    switch (priority) {
      case NotificationPriority.High:
        return 'danger';
      case NotificationPriority.Medium:
        return 'warning';
      case NotificationPriority.Low:
      default:
        return 'neutral';
    }
  }

  getPriorityLabel(priority: NotificationPriority): string {
    switch (priority) {
      case NotificationPriority.High:
        return 'High';
      case NotificationPriority.Medium:
        return 'Medium';
      case NotificationPriority.Low:
        return 'Low';
      default:
        return 'Normal';
    }
  }
}
