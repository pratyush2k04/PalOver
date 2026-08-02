import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult, UserListItem } from '../models/user.model';
import { Role } from '../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class UserDirectoryService {
  private http = inject(HttpClient);

  getUsers(search: string, page = 1, pageSize = 50): Observable<PagedResult<UserListItem>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<PagedResult<UserListItem>>('/api/users', { params });
  }

  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>('/api/roles');
  }

  changeUserRole(userId: number, newRoleId: number): Observable<UserListItem> {
    return this.http.patch<UserListItem>(`/api/users/${userId}/role`, { newRoleId });
  }
}
