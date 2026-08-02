import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { DashboardComponent } from './components/dashboard/dashboard';
import { MainLayoutComponent } from './components/main-layout/main-layout';
import { EngagementsComponent } from './features/engagements/engagements';
import { UsersComponent } from './features/users/users';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },

  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'admin/users', pathMatch: 'full' },
      { path: 'admin/users', component: UsersComponent },//change made for users 
      { path: 'engagements', component: EngagementsComponent },
    ],
  },
];
