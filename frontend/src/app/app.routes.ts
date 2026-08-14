import { Routes } from '@angular/router';
import { TECHNICAL_READINESS_ROUTES, MASTER_CHECKLIST_ROUTES } from '@shared/constants/app.constants';

import { LoginComponent } from './components/login/login';
import { MainLayoutComponent } from '@layouts/main-layout/main-layout';

import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';
import { workspaceGuard } from './guards/workspace.guard';
import { workspaceRedirectGuard } from './guards/workspace-redirect.guard';
// import { engagementAccessGuard } from './core/guards/engagement-access.guard';





export const routes: Routes = [
  // Public
  {
    path: 'login',
    component: LoginComponent,
  },

  // Protected
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],

    children: [
      // Workspace redirect
      {
        path: '',
        pathMatch: 'full',
        canActivate: [workspaceRedirectGuard],
        children: [],
      },

      // // Dashboard
      // {
      //   path: 'dashboard',
      //   component: DashboardComponent,
      // },

      // Role dashboards (Home feature)
      {
        path: '',
        loadChildren: () =>
          import('./features/home/home.routes').then(m => m.HOME_ROUTES),
      },

      // Public/Non-admin Checklist (Read-only)
      {
        path: MASTER_CHECKLIST_ROUTES.PUBLIC_BASE,
        children: [
          {
            path: MASTER_CHECKLIST_ROUTES.PUBLISHED,
            loadComponent: () =>
              import('./features/master-checklist/published/published-checklist').then(
                m => m.PublishedChecklistComponent
              ),
          },
          { path: '', redirectTo: MASTER_CHECKLIST_ROUTES.PUBLISHED, pathMatch: 'full' }
        ],
      },

      // Admin Master Checklist (Tabbed Shell)
      {
        path: MASTER_CHECKLIST_ROUTES.BASE,
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/master-checklist/master-checklist-shell/master-checklist-shell.component').then(
            m => m.MasterChecklistShellComponent
          ),
        children: [
          {
            path: MASTER_CHECKLIST_ROUTES.DRAFT,
            loadComponent: () =>
              import('./features/master-checklist/draft/current-draft').then(
                m => m.CurrentDraftComponent
              ),
          },
          {
            path: MASTER_CHECKLIST_ROUTES.PUBLISHED,
            loadComponent: () =>
              import('./features/master-checklist/published/published-checklist').then(
                m => m.PublishedChecklistComponent
              ),
          },
          {
            path: MASTER_CHECKLIST_ROUTES.HISTORY,
            loadComponent: () =>
              import('./features/master-checklist/history/version-history').then(
                m => m.VersionHistoryComponent
              ),
          },
          { path: '', redirectTo: MASTER_CHECKLIST_ROUTES.DRAFT, pathMatch: 'full' },
        ],
      },

      // Admin
      {
        path: 'admin/users',
        loadChildren: () =>
          import('./features/users/users.routes').then(
            m => m.USERS_ROUTES
          ),
      },
      {
        path: 'engagement-allocation',
        canActivate: [adminGuard],
        loadChildren: () =>
          import('./features/engagement-allocation/engagement-allocation.routes').then(
            m => m.ENGAGEMENT_ALLOCATION_ROUTES
          ),
      },

      // Engagements
      {
        path: 'engagements',
        loadChildren: () =>
          import('./features/engagements/engagements.routes').then(
            m => m.ENGAGEMENTS_ROUTES
          ),
      },
      {
        path: 'my-engagements',
        loadComponent: () =>
          import('./features/engagements/my-engagements/my-engagements').then(
            m => m.MyEngagementsComponent
          ),
      },

      // Portfolio (Leadership)
      {
        path: 'portfolio',
        loadChildren: () =>
          import('./features/portfolio/portfolio.routes').then(
            m => m.PORTFOLIO_ROUTES
          ),
      },

      // Access denied
      {
        path: 'access-denied',
        loadComponent: () =>
          import('./shared/access-denied/access-denied').then(
            m => m.AccessDeniedComponent
          ),
      },
    ],
  },

  // Fallback
  {
    path: '**',
    redirectTo: 'login',
  },
];