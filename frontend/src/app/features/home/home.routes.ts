import { Routes } from '@angular/router';
import { workspaceGuard } from '@shared/guards/workspace.guard';
import { HOME_ROUTES_CONFIG } from '@shared/constants/app.constants';

export const HOME_ROUTES: Routes = [
  {
    path: HOME_ROUTES_CONFIG.ENGAGEMENT_DASHBOARD,
    canActivate: [workspaceGuard],
    loadComponent: () =>
      import('./delivery-home/delivery-home').then(
        (m) => m.DeliveryHomeComponent
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
];
