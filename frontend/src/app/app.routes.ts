import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { MainLayoutComponent } from './components/main-layout/main-layout';
import { EngagementsComponent } from './features/engagements/engagements';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },

  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./components/dashboard/dashboard').then((m) => m.DashboardComponent),
      },
      {
        path: 'checklist',
        children: [
          { path: '', redirectTo: 'published', pathMatch: 'full' },
          {
            path: 'published',
            loadComponent: () =>
              import('./features/checklist/published/published-checklist').then(
                (m) => m.PublishedChecklistComponent
              ),
          },
          {
            path: 'draft',
            canActivate: [adminGuard],
            loadComponent: () =>
              import('./features/checklist/draft/current-draft').then(
                (m) => m.CurrentDraftComponent
              ),
          },
          {
            path: 'history',
            canActivate: [adminGuard],
            loadComponent: () =>
              import('./features/checklist/history/version-history').then(
                (m) => m.VersionHistoryComponent
              ),
          },
        ],
      },
      {
        path: 'access-denied',
        loadComponent: () =>
          import('./features/access-denied/access-denied').then((m) => m.AccessDeniedComponent),
      },
    ],
  },

  { path: '**', redirectTo: 'dashboard' },
];
