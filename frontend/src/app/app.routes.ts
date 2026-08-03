import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { DashboardComponent } from './components/dashboard/dashboard';
import { MainLayoutComponent } from './components/main-layout/main-layout';
import { engagementAccessGuard } from './core/guards/engagement-access.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },

  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'admin/users', pathMatch: 'full' },
      {
        path: 'engagements/:id/workspace',
        canActivate: [engagementAccessGuard],
        loadComponent: () => import('./features/engagement-workspace/engagement-workspace.component').then(m => m.EngagementWorkspaceComponent),
        children: [
          // Handover will be added here in Branch 5
        ]
      }
    ],
  },
];
