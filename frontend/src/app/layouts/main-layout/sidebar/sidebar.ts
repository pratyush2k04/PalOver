import { Component, inject, computed } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AppContextService } from '@shared/services/app-context';
import { AuthService } from '@shared/services/auth';
import { APP_ROUTES, USER_ROLES, MASTER_CHECKLIST_ROUTES } from '@shared/constants/app.constants';
import { LayoutService } from '@shared/services/layout.service';

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
  readonly layoutService = inject(LayoutService);

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

    // Users, Engagement Allocation, Master Checklist and Audit Log links: visible ONLY to Admin
    const isAdminUser = currentUser.isAdmin || currentUser.role === USER_ROLES.ADMIN || this.authService.isAdmin();
    if (isAdminUser) {
      items.push({ label: 'Users', icon: 'users', route: APP_ROUTES.USERS });
      items.push({ label: 'Engagement Allocation', icon: 'user-check', route: APP_ROUTES.ENGAGEMENT_ALLOCATION });
      items.push({ label: 'Master Checklist', icon: 'checklist', route: '/' + MASTER_CHECKLIST_ROUTES.BASE });
      items.push({ label: 'Audit Log', icon: 'shield', route: APP_ROUTES.AUDIT_LOG });
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

    // New Portfolio navigation item: visible ONLY to Leadership
    if (currentUser.role === USER_ROLES.LEADERSHIP) {
      items.push({
        label: 'Portfolio',
        icon: 'target',
        route: APP_ROUTES.PORTFOLIO
      });
    }

    return items;
  });
}
