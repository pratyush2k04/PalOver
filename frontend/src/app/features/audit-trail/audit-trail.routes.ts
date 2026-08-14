import { Routes } from '@angular/router';

export const AUDIT_TRAIL_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./audit-trail').then((m) => m.AuditTrailComponent)
  }
];
