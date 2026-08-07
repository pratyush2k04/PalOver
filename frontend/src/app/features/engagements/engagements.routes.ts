import { Routes } from '@angular/router';

export const ENGAGEMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./my-engagements').then((m) => m.MyEngagementsComponent),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('../engagement-detail/engagement-detail').then(
        (m) => m.EngagementDetailComponent
      ),
  },
  {
    path: ':id/workspace',
    loadComponent: () =>
      import(
        './engagement-workspace/engagement-workspace.component'
      ).then((m) => m.EngagementWorkspaceComponent),
    children: [
      {
        path: 'handover',
        loadComponent: () =>
          import('./engagement-workspace/handover/handover.component').then(
            (m) => m.HandoverComponent
          ),
      },
      {
        path: 'documents',
        loadComponent: () =>
          import(
            '../documents/engagement-documents/engagement-documents.component'
          ).then((m) => m.EngagementDocumentsComponent),
      },
      {
        path: 'checklist',
        loadComponent: () =>
          import(
            './engagement-workspace/checklist/engagement-checklist.component'
          ).then((m) => m.EngagementChecklistComponent),
      },
      {
        path: 'kickoff',
        loadComponent: () =>
          import('./engagement-workspace/kickoff/kickoff.component').then(
            (m) => m.KickoffComponent
          ),
      },
      {
        path: '',
        redirectTo: 'handover',
        pathMatch: 'full',
      },
    ],
  },
];
