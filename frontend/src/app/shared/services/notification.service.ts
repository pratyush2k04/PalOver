import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Notification, PagedResult } from '../models/notification.model';
import { NOTIFICATIONS_API_ENDPOINTS } from '../constants/notifications.constants';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);

  getNotifications(page: number, pageSize: number): Observable<PagedResult<Notification>> {
    return this.http.get<PagedResult<Notification>>(NOTIFICATIONS_API_ENDPOINTS.NOTIFICATIONS, {
      params: { page, pageSize }
    });
  }

  getUnreadCount(): Observable<number> {
    return this.http.get<number>(NOTIFICATIONS_API_ENDPOINTS.UNREAD_COUNT);
  }

  markAsRead(id: number): Observable<void> {
    return this.http.put<void>(NOTIFICATIONS_API_ENDPOINTS.MARK_AS_READ(id), {});
  }

  markAllAsRead(): Observable<void> {
    return this.http.put<void>(NOTIFICATIONS_API_ENDPOINTS.MARK_ALL_AS_READ, {});
  }
}
