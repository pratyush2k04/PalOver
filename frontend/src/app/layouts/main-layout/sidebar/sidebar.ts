import { Component, inject, computed } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AppContextService } from '../../../services/app-context';
import { AuthService } from '../../../services/auth';

export interface NavItem {
  label: string;
  icon: string;
  route?: string;
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

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;

  readonly navItems = computed<NavItem[]>(() => {
    const currentUser = this.user();
    if (!currentUser) return [];

    const ws = this.workspace();
    const homeRoute = ws?.route ?? '/dashboard';

    const items: NavItem[] = [
      { label: 'Home', icon: 'home', route: homeRoute }
    ];

    // Engagements link: visible ONLY to Pre Sales
    if (currentUser.role === 'Pre Sales') {
      items.push({ label: 'Engagements', icon: 'layers', route: '/engagements' });
    }

    // Role Queue link (route: /my-engagements): hidden for Admin
    if (!currentUser.isAdmin && currentUser.role !== 'Admin') {
      let queueLabel = 'My\nEngagements';
      let icon = 'briefcase';

      if (currentUser.role === 'Tech Lead') {
        queueLabel = 'Tech\nQueue';
        icon = 'briefcase';
      } else if (currentUser.role === 'Pre Sales') {
        queueLabel = 'My\nHandovers';
        icon = 'clipboard';
      } else if (currentUser.role === 'Project Management' || currentUser.role === 'Leadership') {
        queueLabel = 'My\nEngagements';
        icon = 'briefcase';
      }

      items.push({
        label: queueLabel,
        icon: icon,
        route: '/my-engagements'
      });
    }

    // Alerts and Profile: no route assigned
    items.push(
      { label: 'Alerts', icon: 'bell', badge: 'dot' },
      { label: 'Profile', icon: 'user' }
    );

    return items;
  });

  onSignOut(): void {
    this.authService.logout();
    this.appContext.reset();
    this.router.navigate(['/login']);
  }
}
