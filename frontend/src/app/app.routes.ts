import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { DashboardComponent } from './components/dashboard/dashboard';
import { MainLayoutComponent } from './components/main-layout/main-layout';
import { EngagementsComponent } from './features/engagements/engagements';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },

  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'admin/users', pathMatch: 'full' },
      { path: 'engagements', component: EngagementsComponent },
      {
        path: 'my-engagements',
        loadComponent: () =>
          import('./features/home/project-manager-home/project-manager-home').then(
            (m) => m.ProjectManagerHomeComponent
          ),
      },
      {
        path: 'my-handovers',
        loadComponent: () =>
          import('./features/home/pre-sales-home/pre-sales-home').then((m) => m.PreSalesHomeComponent),
      },
      {
        path: 'technical-readiness',
        loadComponent: () =>
          import('./features/home/tech-lead-home/tech-lead-home').then((m) => m.TechLeadHomeComponent),
      },
      {
        path: 'portfolio-dashboard',
        loadComponent: () =>
          import('./features/home/leadership-home/leadership-home').then((m) => m.LeadershipHomeComponent),
      },
      {
        path: 'user-directory',
        loadComponent: () =>
          import('./features/home/admin-home/admin-home').then((m) => m.AdminHomeComponent),
      },
    ],
  },

  { path: '**', redirectTo: 'login' },
];
