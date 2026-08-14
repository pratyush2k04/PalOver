import { Routes } from '@angular/router';

export const PORTFOLIO_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./portfolio-dashboard/portfolio-dashboard').then(
        (m) => m.PortfolioDashboardComponent
      ),
  },
];
