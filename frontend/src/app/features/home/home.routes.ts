import { Routes } from '@angular/router';
import { workspaceGuard } from '../../guards/workspace.guard';
import { HOME_ROUTES_CONFIG } from '../../constants/app.constants';

export const HOME_ROUTES: Routes = [
  {
    path: HOME_ROUTES_CONFIG.ENGAGEMENT_DASHBOARD,
    canActivate: [workspaceGuard],
    loadComponent: () =>
      import('./project-manager-home/project-manager-home').then(
        (m) => m.ProjectManagerHomeComponent
      ),
  },
  {
    path: HOME_ROUTES_CONFIG.MY_HANDOVERS,
    canActivate: [workspaceGuard],
    loadComponent: () =>
      import('./pre-sales-home/pre-sales-home').then(
        (m) => m.PreSalesHomeComponent
      ),
  },
  {
    path: HOME_ROUTES_CONFIG.TECHNICAL_READINESS,
    canActivate: [workspaceGuard],
    loadComponent: () =>
      import('./tech-lead-home/tech-lead-home').then(
        (m) => m.TechLeadHomeComponent
      ),
  },
  {
    path: HOME_ROUTES_CONFIG.PORTFOLIO_DASHBOARD,
    canActivate: [workspaceGuard],
    loadComponent: () =>
      import('./leadership-home/leadership-home').then(
        (m) => m.LeadershipHomeComponent
      ),
  },
  {
    path: HOME_ROUTES_CONFIG.USER_DIRECTORY,
    canActivate: [workspaceGuard],
    loadComponent: () =>
      import('./admin-home/admin-home').then((m) => m.AdminHomeComponent),
  },
];
