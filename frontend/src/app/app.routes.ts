import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login';
import { MainLayoutComponent } from './layouts/main-layout/main-layout';

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

      // Role dashboards (Home feature)
      {
        path: '',
        loadChildren: () =>
          import('./features/home/home.routes').then(m => m.HOME_ROUTES),
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
        loadChildren: () =>
          import('./features/users/users.routes').then(
            m => m.USERS_ROUTES
          ),
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