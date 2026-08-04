import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login';
import { MainLayoutComponent } from './components/main-layout/main-layout';

import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';
import { workspaceGuard } from './guards/workspace.guard';
import { workspaceRedirectGuard } from './guards/workspace-redirect.guard';
// import { engagementAccessGuard } from './core/guards/engagement-access.guard';

import { DashboardComponent } from './components/dashboard/dashboard';

import { EngagementsComponent } from './features/engagements/engagements';
import { EngagementDetailComponent } from './features/engagement-detail/engagement-detail';
import { UsersComponent } from './features/users/users';

const checklistRoutes: Routes = [
  { path: '', redirectTo: 'published', pathMatch: 'full' },

  {
    path: 'published',
    loadComponent: () =>
      import('./features/checklist/published/published-checklist').then(
        m => m.PublishedChecklistComponent
      ),
  },

  {
    path: 'draft',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/checklist/draft/current-draft').then(
        m => m.CurrentDraftComponent
      ),
  },

  {
    path: 'history',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/checklist/history/version-history').then(
        m => m.VersionHistoryComponent
      ),
  },
];

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

      // Dashboard
      {
        path: 'dashboard',
        component: DashboardComponent,
      },

      // Role dashboards
      {
        path: 'engagement-dashboard',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/project-manager-home/project-manager-home').then(
            m => m.ProjectManagerHomeComponent
          ),
      },
      {
        path: 'my-handovers',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/pre-sales-home/pre-sales-home').then(
            m => m.PreSalesHomeComponent
          ),
      },
      {
        path: 'technical-readiness',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/tech-lead-home/tech-lead-home').then(
            m => m.TechLeadHomeComponent
          ),
      },
      {
        path: 'portfolio-dashboard',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/leadership-home/leadership-home').then(
            m => m.LeadershipHomeComponent
          ),
      },
      {
        path: 'user-directory',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/admin-home/admin-home').then(
            m => m.AdminHomeComponent
          ),
      },

      // Checklist
      {
        path: 'checklist',
        children: checklistRoutes,
      },

      {
        path: 'master-checklist',
        children: checklistRoutes,
      },

      // Admin
      {
        path: 'admin/users',
        component: UsersComponent,
      },

      // Engagements
      {
        path: 'engagements',
        component: EngagementsComponent,
      },
      {
        path: 'engagements/:id',
        component: EngagementDetailComponent,
      },
      {
        path: 'engagements/:id/workspace',
        // canActivate: [engagementAccessGuard],
        loadComponent: () =>
          import(
            './features/engagement-workspace/engagement-workspace.component'
          ).then(m => m.EngagementWorkspaceComponent),

        children: [
          {
            path: 'handover',
            loadComponent: () =>
              import(
                './features/engagement-workspace/handover/handover.component'
              ).then(m => m.HandoverComponent),
          },
          {
            path: 'documents',
            loadComponent: () =>
              import(
                './features/documents/engagement-documents/engagement-documents.component'
              ).then(m => m.EngagementDocumentsComponent),
          },
          {
            path: '',
            redirectTo: 'handover',
            pathMatch: 'full',
          },
        ],
      },

      // Access denied
      {
        path: 'access-denied',
        loadComponent: () =>
          import('./features/access-denied/access-denied').then(
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