import { Routes } from '@angular/router';
import { TECHNICAL_READINESS_ROUTES, MASTER_CHECKLIST_ROUTES } from './constants/app.constants';

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
import { MyEngagementsComponent } from './features/my-engagements/my-engagements';



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
        path: TECHNICAL_READINESS_ROUTES.BASE,
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
            path: 'checklist',
            loadComponent: () =>
              import(
                './features/engagement-workspace/checklist/engagement-checklist.component'
              ).then(m => m.EngagementChecklistComponent),
          },
          {
            path: 'kickoff',
            loadComponent: () =>
              import(
                './features/engagement-workspace/kickoff/kickoff.component'
              ).then(m => m.KickoffComponent),
          },
          {
            path: TECHNICAL_READINESS_ROUTES.BASE,
            loadComponent: () =>
              import(
                './features/engagements/technical/technical-readiness.component'
              ).then(m => m.TechnicalReadinessComponent),
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
      { path: '', redirectTo: 'admin/users', pathMatch: 'full' },
      { path: 'admin/users', component: UsersComponent },//change made for users
      { path: 'engagements', component: EngagementsComponent },
      { path: 'engagements/:id', component: EngagementDetailComponent },//change made for leader to assign pm-tl
      {
        path: 'engagements/:id/workspace',
        //canActivate: [engagementAccessGuard],
        loadComponent: () => import('./features/engagement-workspace/engagement-workspace.component').then(m => m.EngagementWorkspaceComponent),
        children: [
          { 
            path: 'handover', 
            loadComponent: () => import('./features/engagement-workspace/handover/handover.component').then(m => m.HandoverComponent) 
          },
          { 
            path: 'documents', 
            loadComponent: () => import('./features/documents/engagement-documents/engagement-documents.component').then(m => m.EngagementDocumentsComponent) 
          },
          {
            path: 'checklist',
            loadComponent: () => import('./features/engagement-workspace/checklist/engagement-checklist.component').then(m => m.EngagementChecklistComponent)
          },
          {
            path: 'kickoff',
            loadComponent: () => import('./features/engagement-workspace/kickoff/kickoff.component').then(m => m.KickoffComponent)
          },
          {
            path: TECHNICAL_READINESS_ROUTES.BASE,
            loadComponent: () => import('./features/engagements/technical/technical-readiness.component').then(m => m.TechnicalReadinessComponent)
          },
          { path: '', redirectTo: 'handover', pathMatch: 'full' }
        ]
      },
      { path: 'my-engagements', component: MyEngagementsComponent },
    ],
  },

  // Fallback
  {
    path: '**',
    redirectTo: 'login',
  },
];