import { Routes } from '@angular/router';

export const ENGAGEMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./my-engagements').then((m) => m.MyEngagementsComponent),
  },
  {
    path: 'my-engagements',
    loadComponent: () =>
      import('./my-engagements').then((m) => m.MyEngagementsComponent),
  },
];
