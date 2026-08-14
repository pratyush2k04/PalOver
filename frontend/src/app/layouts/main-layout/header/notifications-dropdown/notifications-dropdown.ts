import { Component, ElementRef, HostListener, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { ButtonComponent } from '@common/components/button/button';
import { NotificationService } from '../../../../shared/services/notification.service';
import { Notification } from '../../../../shared/models/notification.model';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-notifications-dropdown',
  standalone: true,
  imports: [InlineSVGModule, EmptyStateComponent, LoadingSpinnerComponent, ButtonComponent],
  templateUrl: './notifications-dropdown.html',
  styleUrl: './notifications-dropdown.scss'
})
export class NotificationsDropdownComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private elementRef = inject(ElementRef);

  readonly isOpen = signal(false);
  readonly notifications = signal<Notification[]>([]);
  readonly unreadCount = signal(0);
  readonly loading = signal(false);

  ngOnInit(): void {
    this.notificationService.getUnreadCount().subscribe(count => this.unreadCount.set(count));
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

  markAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.markLocallyRead(this.notifications().map(n => n.id));
      this.unreadCount.set(0);
    });
  }

  private markLocallyRead(ids: number[]): void {
    const idSet = new Set(ids);
    this.notifications.update(items =>
      items.map(item => (idSet.has(item.id) ? { ...item, readAt: new Date().toISOString(), status: 'Read' } : item))
    );
  }

  private loadNotifications(): void {
    this.loading.set(true);
    this.notificationService.getNotifications(1, PAGE_SIZE).subscribe(result => {
      this.notifications.set(result.items);
      this.loading.set(false);
    });
  }
}
