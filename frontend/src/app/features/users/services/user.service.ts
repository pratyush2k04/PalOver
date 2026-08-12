import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult, UserListItem } from '../models/user.model';
import { Role } from '../models/role.model';
import { API_ENDPOINTS } from '@shared/constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);

  getUsers(search: string, page = 1, pageSize = 50): Observable<PagedResult<UserListItem>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<PagedResult<UserListItem>>(API_ENDPOINTS.USERS, { params });
  }

  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(API_ENDPOINTS.ROLES);
  }

  getUsersInRole(roleId: number): Observable<UserListItem[]> {
    return this.http.get<UserListItem[]>(API_ENDPOINTS.ROLE_USERS(roleId));
  }

  changeUserRole(userId: number, newRoleId: number): Observable<UserListItem> {
    return this.http.patch<UserListItem>(API_ENDPOINTS.USER_ROLE(userId), { newRoleId });
  }
}

// Export alias for compatibility
export { UserService as UserDirectoryService };
