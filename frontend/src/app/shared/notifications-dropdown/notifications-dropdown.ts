import { Component, DestroyRef, ElementRef, HostListener, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { switchMap, timer } from 'rxjs';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { ButtonComponent } from '@common/components/button/button';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { NotificationService } from '../services/notification.service';
import { Notification } from '../models/notification.model';
import { APP_ROUTES } from '@shared/constants/app.constants';

const PAGE_SIZE = 5;
const UNREAD_COUNT_POLL_INTERVAL_MS = 30_000;

@Component({
  selector: 'app-notifications-dropdown',
  standalone: true,
  imports: [InlineSVGModule, EmptyStateComponent, LoadingSpinnerComponent, ButtonComponent, TooltipDirective],
  templateUrl: './notifications-dropdown.html',
  styleUrl: './notifications-dropdown.scss'
})
export class NotificationsDropdownComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private elementRef = inject(ElementRef);
  private destroyRef = inject(DestroyRef);

  readonly isOpen = signal(false);
  readonly notifications = signal<Notification[]>([]);
  readonly unreadCount = signal(0);
  readonly loading = signal(false);

  ngOnInit(): void {
    timer(0, UNREAD_COUNT_POLL_INTERVAL_MS)
      .pipe(
        switchMap(() => this.notificationService.getUnreadCount()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(count => {
        const previousCount = this.unreadCount();
        this.unreadCount.set(count);

        if (this.isOpen() && count > previousCount) {
          this.loadNotifications(false);
        }
      });
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.isOpen() && !this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }

  toggle(): void {
    this.isOpen.update(open => !open);
    if (this.isOpen() && this.notifications().length === 0) {
      this.loadNotifications();
    }
  }

  onNotificationClick(notification: Notification): void {
    if (!notification.readAt) {
      this.notificationService.markAsRead(notification.id).subscribe(() => {
        this.markLocallyRead([notification.id]);
        this.unreadCount.update(count => Math.max(0, count - 1));
      });
    }

    this.isOpen.set(false);
    if (notification.link) {
      this.router.navigateByUrl(notification.link);
    }
  }

  onViewAllClick(): void {
    this.isOpen.set(false);
    this.router.navigate([APP_ROUTES.NOTIFICATIONS]);
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications.set([]);
      this.unreadCount.set(0);
    });
  }

  private markLocallyRead(ids: number[]): void {
    const idSet = new Set(ids);
    this.notifications.update(items => items.filter(item => !idSet.has(item.id)));
  }

  private loadNotifications(showLoading = true): void {
    if (showLoading) {
      this.loading.set(true);
    }
    this.notificationService.getNotifications(undefined, 1, 50).subscribe(result => {
      const unreadRecent = result.items.filter(n => !n.readAt).slice(0, 5);
      this.notifications.set(unreadRecent);
      this.loading.set(false);
    });
  }
}
