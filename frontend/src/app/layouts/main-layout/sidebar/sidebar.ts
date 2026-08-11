import { Component, inject, computed } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AppContextService } from '../../../services/app-context';
import { AuthService } from '../../../services/auth';
import { APP_ROUTES, USER_ROLES, MASTER_CHECKLIST_ROUTES } from '@shared/constants/app.constants';

import { InlineSVGModule } from 'ng-inline-svg-2';

export interface NavItem {
  label: string;
  icon: string;
  route?: string;
  badge?: number | 'dot';
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, InlineSVGModule],
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

    // Users, Engagement Allocation and Master Checklist links: visible ONLY to Admin
    const isAdminUser = currentUser.isAdmin || currentUser.role === USER_ROLES.ADMIN || this.authService.isAdmin();
    if (isAdminUser) {
      items.push({ label: 'Users', icon: 'users', route: APP_ROUTES.USERS });
      items.push({ label: 'Engagement Allocation', icon: 'user-check', route: APP_ROUTES.ENGAGEMENT_ALLOCATION });
      items.push({ label: 'Master Checklist', icon: 'checklist', route: '/' + MASTER_CHECKLIST_ROUTES.BASE });
    }


    // Role Queue link (route: /my-engagements): shown for non-Admin roles regardless of isAdmin flag
    if (currentUser.role !== USER_ROLES.ADMIN) {
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
