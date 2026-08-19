import { Routes } from '@angular/router';
import { RISK_ROUTES } from '@shared/constants/app.constants';

export const ENGAGEMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./my-engagements/my-engagements').then((m) => m.MyEngagementsComponent),
  },
  {
    path: ':id',
    redirectTo: ':id/workspace/overview',
    pathMatch: 'full',
  },
  {
    path: ':id/workspace',
    loadComponent: () =>
      import(
        './engagement-workspace/engagement-workspace.component'
      ).then((m) => m.EngagementWorkspaceComponent),
    children: [
      {
        path: 'overview',
        loadComponent: () =>
          import(
            './engagement-workspace/overview/overview.component'
          ).then((m) => m.OverviewComponent),
      },
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
            './engagement-workspace/documents/engagement-documents/engagement-documents.component'
          ).then((m) => m.EngagementDocumentsComponent),
      },
      {
        path: 'people',
        loadComponent: () =>
          import('./engagement-workspace/people/people.component').then(
            (m) => m.PeopleComponent
          ),
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
        path: 'technical-readiness',
        loadComponent: () =>
          import('./engagement-workspace/technical/technical-readiness.component').then(
            (m) => m.TechnicalReadinessComponent
          ),
      },
      {
        path: RISK_ROUTES.BASE,
        loadComponent: () =>
          import('./engagement-workspace/risks/risks.component').then(
            (m) => m.RisksComponent
          ),
      },
      {
        path: 'audits',
        loadComponent: () =>
          import(
            './engagement-workspace/audits/engagement-audits/engagement-audits.component'
          ).then((m) => m.EngagementAuditsComponent),
      },
      {
        path: '',
        redirectTo: 'overview',
        pathMatch: 'full',
      },
    ],
  },
];
