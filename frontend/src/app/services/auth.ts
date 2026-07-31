import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface User {
  fullName: string;
  email: string;
  role: string;
  isAdmin: boolean;
}

const TOKEN_KEY = 'pdr_jwt_token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);

  private readonly currentUserSignal = signal<User | null>(this.readUserFromToken());
  readonly currentUser = this.currentUserSignal.asReadonly();

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('/api/auth/login', credentials).pipe(
      tap((response) => {
        if (response && response.token) {
          localStorage.setItem(TOKEN_KEY, response.token);
          this.currentUserSignal.set(this.readUserFromToken());
        }
      })
    );
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  clearToken(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.currentUserSignal.set(null);
  }

  logout(): void {
    this.clearToken();
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    const payload = this.decodeToken(token);
    return payload?.['isAdmin'] === true || payload?.['isAdmin'] === 'true';
  }

  private readUserFromToken(): User | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }

    const payload = this.decodeToken(token);
    if (!payload) {
      return null;
    }

    return {
      fullName: (payload['fullName'] as string) ?? '',
      email: (payload['email'] as string) ?? '',
      role: (payload['role'] as string) ?? '',
      isAdmin: payload['isAdmin'] === true || payload['isAdmin'] === 'true',
    };
  }

  private decodeToken(token: string): Record<string, unknown> | null {
    try {
      const payload = token.split('.')[1];
      const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(json);
    } catch {
      return null;
    }
  }
}
