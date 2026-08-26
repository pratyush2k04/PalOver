import { Routes } from '@angular/router';

export const NOTIFICATION_FULLPAGE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./notification-fullpage.component').then((m) => m.NotificationFullpageComponent)
  }
];
