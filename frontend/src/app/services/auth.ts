import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);

  login(credentials: LoginRequest): Observable<void> {
    return this.http.post<void>('/api/auth/login', credentials, {
      withCredentials: true
    });
  }

  logout(): Observable<void> {
    return this.http.post<void>('/api/auth/logout', {}, {
      withCredentials: true
    });
  }
}
