import { Routes } from '@angular/router';

export const AUDIT_LOG_ROUTE: Routes = [
  {
    path: '',
    loadComponent: () => import('./audit-log').then((m) => m.AuditLogComponent)
  }
];
