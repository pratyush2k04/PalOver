import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div style="padding: 2.5rem; text-align: center; font-family: system-ui, sans-serif;">
      <h2>Dashboard Placeholder</h2>
      <p>Authentication successful. You have reached the dashboard.</p>
      <button 
        (click)="onLogout()" 
        style="margin-top: 1.5rem; padding: 0.6rem 1.2rem; background: #dc2626; color: white; border: none; border-radius: 4px; cursor: pointer;">
        Sign Out
      </button>
    </div>
  `
})
export class DashboardComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  onLogout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: () => {
        this.router.navigate(['/login']);
      }
    });
  }
}
