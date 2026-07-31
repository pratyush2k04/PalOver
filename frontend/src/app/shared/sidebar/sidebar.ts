import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

interface NavItem {
  label: string;
  icon: string;
  badge?: number | 'dot';
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  readonly navItems: NavItem[] = [
    { label: 'Home', icon: 'home' },
    { label: 'My Handovers', icon: 'clipboard' },
    { label: 'Alerts', icon: 'bell', badge: 'dot' },
    { label: 'Profile', icon: 'user' }
  ];

  private readonly glyphs: Record<string, string> = {
    home: '⌂',
    layers: '▤',
    clipboard: '▥',
    bell: '🔔',
    user: '◍',
    'sign-out': '⇥'
  };

  iconGlyph(icon: string): string {
    return this.glyphs[icon] ?? '•';
  }

  onSignOut(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
