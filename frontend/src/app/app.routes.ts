import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login';
import { MainLayoutComponent } from './layouts/main-layout/main-layout';

import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';
import { workspaceGuard } from './guards/workspace.guard';
import { workspaceRedirectGuard } from './guards/workspace-redirect.guard';
// import { engagementAccessGuard } from './core/guards/engagement-access.guard';



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
        loadChildren: () =>
          import('./features/engagements/engagements.routes').then(
            m => m.ENGAGEMENTS_ROUTES
          ),
      },
      {
        path: 'my-engagements',
        loadComponent: () =>
          import('./features/engagements/my-engagements').then(
            m => m.MyEngagementsComponent
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