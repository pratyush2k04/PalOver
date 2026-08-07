import { Component, inject, computed } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AppContextService } from '../../../services/app-context';
import { AuthService } from '../../../services/auth';
import { APP_ROUTES, USER_ROLES } from '../../../constants/app.constants';

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
    const homeRoute = ws?.route ?? APP_ROUTES.DASHBOARD;

    const items: NavItem[] = [
      { label: 'Home', icon: 'home', route: homeRoute }
    ];

    // Users link: visible ONLY to Admin
    if (currentUser.isAdmin || currentUser.role === USER_ROLES.ADMIN) {
      items.push({ label: 'Users', icon: 'users', route: APP_ROUTES.USERS });
    }

    // Engagements link: visible ONLY to Pre Sales
    if (currentUser.role === USER_ROLES.PRE_SALES) {
      items.push({ label: 'Engagements', icon: 'layers', route: APP_ROUTES.ENGAGEMENTS });
    }

    // Role Queue link (route: /my-engagements): hidden for Admin
    if (!currentUser.isAdmin && currentUser.role !== USER_ROLES.ADMIN) {
      let queueLabel = 'My\nEngagements';
      let icon = 'briefcase';

      if (currentUser.role === USER_ROLES.TECH_LEAD) {
        queueLabel = 'Tech\nQueue';
        icon = 'briefcase';
      } else if (currentUser.role === USER_ROLES.PRE_SALES) {
        queueLabel = 'My\nHandovers';
        icon = 'clipboard';
      } else if (currentUser.role === USER_ROLES.PROJECT_MANAGEMENT || currentUser.role === USER_ROLES.LEADERSHIP) {
        queueLabel = 'My\nEngagements';
        icon = 'briefcase';
      }

      items.push({
        label: queueLabel,
        icon: icon,
        route: APP_ROUTES.MY_ENGAGEMENTS
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
    this.router.navigate([APP_ROUTES.LOGIN]);
  }
}
