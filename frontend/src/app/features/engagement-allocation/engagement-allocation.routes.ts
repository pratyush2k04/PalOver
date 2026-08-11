import { Routes } from '@angular/router';

export const ENGAGEMENT_ALLOCATION_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./engagement-allocation.component').then(
        m => m.EngagementAllocationComponent
      ),
  },
];
