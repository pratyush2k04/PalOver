import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Notification, PagedResult } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);

  getNotifications(page: number, pageSize: number): Observable<PagedResult<Notification>> {
    return this.http.get<PagedResult<Notification>>('/api/notifications', {
      params: { page, pageSize }
    });
  }

  getUnreadCount(): Observable<number> {
    return this.http.get<number>('/api/notifications/unread-count');
  }

  markAsRead(id: number): Observable<void> {
    return this.http.put<void>(`/api/notifications/${id}/read`, {});
  }

  markAllAsRead(): Observable<void> {
    return this.http.put<void>('/api/notifications/read-all', {});
  }
}
