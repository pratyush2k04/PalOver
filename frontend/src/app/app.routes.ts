import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { DashboardComponent } from './components/dashboard/dashboard';
import { MainLayoutComponent } from './components/main-layout/main-layout';
import { EngagementsComponent } from './features/engagements/engagements';
import { MyEngagementsComponent } from './features/my-engagements/my-engagements';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },

  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'admin/users', pathMatch: 'full' },
      { path: 'engagements', component: EngagementsComponent },
      { path: 'my-engagements', component: MyEngagementsComponent },
    ],
  },
];
