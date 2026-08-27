import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Notification, PagedResult } from '../models/notification.model';
import { NOTIFICATIONS_API_ENDPOINTS } from '../constants/notification.constants';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);

  getNotifications(engagementId?: number, page = 1, pageSize = 20): Observable<PagedResult<Notification>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (engagementId !== undefined && engagementId !== null) {
      params = params.set('engagementId', engagementId.toString());
    }

    return this.http.get<PagedResult<Notification>>(NOTIFICATIONS_API_ENDPOINTS.NOTIFICATIONS, { params });
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

  bulkDelete(ids: number[]): Observable<void> {
    return this.http.delete<void>(NOTIFICATIONS_API_ENDPOINTS.NOTIFICATIONS, {
      body: { ids }
    });
  }
}
