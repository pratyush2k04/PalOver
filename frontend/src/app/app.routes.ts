import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { MainLayoutComponent } from './components/main-layout/main-layout';
import { engagementAccessGuard } from './core/guards/engagement-access.guard';
import { EngagementsComponent } from './features/engagements/engagements';
import { adminGuard } from './guards/admin.guard';
import { authGuard } from './guards/auth.guard';
import { workspaceGuard } from './guards/workspace.guard';
import { workspaceRedirectGuard } from './guards/workspace-redirect.guard';
import { EngagementDetailComponent } from './features/engagement-detail/engagement-detail';
import { UsersComponent } from './features/users/users';
import { MyEngagementsComponent } from './features/my-engagements/my-engagements';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },

  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', canActivate: [workspaceRedirectGuard], children: [] },
      { path: 'engagements', component: EngagementsComponent },

      {
        path: 'engagement-dashboard',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/project-manager-home/project-manager-home').then(
            (m) => m.ProjectManagerHomeComponent
          ),
      },
      {
        path: 'my-handovers',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/pre-sales-home/pre-sales-home').then((m) => m.PreSalesHomeComponent),
      },
      {
        path: 'technical-readiness',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/tech-lead-home/tech-lead-home').then((m) => m.TechLeadHomeComponent),
      },
      {
        path: 'portfolio-dashboard',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/leadership-home/leadership-home').then((m) => m.LeadershipHomeComponent),
      },
      {
        path: 'user-directory',
        canActivate: [workspaceGuard],
        loadComponent: () =>
          import('./features/home/admin-home/admin-home').then((m) => m.AdminHomeComponent),
      },
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
          { path: '', redirectTo: 'handover', pathMatch: 'full' }
        ]
      },
      { path: 'my-engagements', component: MyEngagementsComponent },
    ],
  },

  { path: '**', redirectTo: 'login' },
];
