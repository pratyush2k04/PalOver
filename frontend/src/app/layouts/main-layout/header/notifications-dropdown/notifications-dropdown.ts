import { Component, ElementRef, HostListener, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { ButtonComponent } from '@common/components/button/button';
import { NotificationService } from '../../../../services/notification.service';
import { Notification } from '../../../../models/notification.model';

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
  readonly hasMore = signal(false);

  private page = 1;

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
      this.loadPage(1);
    }
  }

  loadMore(): void {
    this.loadPage(this.page + 1);
  }

  onNotificationClick(notification: Notification): void {
    this.isOpen.set(false);
    if (notification.link) {
      this.router.navigateByUrl(notification.link);
    }
  }

  private loadPage(page: number): void {
    this.loading.set(true);
    this.notificationService.getNotifications(page, PAGE_SIZE).subscribe(result => {
      this.notifications.update(existing => (page === 1 ? result.items : [...existing, ...result.items]));
      this.page = result.page;
      this.hasMore.set(result.page * result.pageSize < result.totalCount);
      this.loading.set(false);
    });
  }
}
