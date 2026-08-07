import { Component, inject, computed } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AppContextService } from '../../services/app-context';
import { AuthService } from '../../services/auth';


interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: number | 'dot';
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  private appContext = inject(AppContextService);
  private authService = inject(AuthService);
  private router = inject(Router);

  readonly workspace = this.appContext.workspace;

  readonly navItems = computed(() => {
    const items: NavItem[] = [
      { label: 'Home', icon: 'home', route: '/dashboard' },
      { label: 'Engagements', icon: 'layers', route: '/engagements' },
      { label: 'My Engagements', icon: 'briefcase', route: '/my-engagements' },
      { label: 'My Handovers', icon: 'clipboard', route: '/dashboard' },
      { label: 'Alerts', icon: 'bell', route: '/dashboard', badge: 'dot' },
      { label: 'Profile', icon: 'user', route: '/dashboard' }
    ];

    if (this.authService.currentUser()?.isAdmin) {
      items.push({ label: 'Master Checklist', icon: 'checklist', route: '/master-checklist' });
    }

    return items;
  });

  private readonly glyphs: Record<string, string> = {
    home: '⌂',
    layers: '▤',
    briefcase: '💼',
    clipboard: '▥',
    bell: '🔔',
    user: '◍',
    'sign-out': '⇥',
    checklist: '☑'
  };

  iconGlyph(icon: string): string {
    return this.glyphs[icon] ?? '•';
  }

  onSignOut(): void {
    this.authService.logout();
    this.appContext.reset();
    this.router.navigate(['/login']);
  }
}
