import { Routes } from '@angular/router';
import { ENGAGEMENT_ROUTES, TECHNICAL_READINESS_ROUTES, RISK_ROUTES } from '@shared/constants/app.constants';

export const ENGAGEMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./my-engagements/my-engagements').then((m) => m.MyEngagementsComponent),
  },
  {
    path: ':id',
    redirectTo: `:id/${ENGAGEMENT_ROUTES.WORKSPACE}/${ENGAGEMENT_ROUTES.OVERVIEW}`,
    pathMatch: 'full',
  },
  {
    path: `:id/${ENGAGEMENT_ROUTES.WORKSPACE}`,
    loadComponent: () =>
      import(
        './engagement-workspace/engagement-workspace.component'
      ).then((m) => m.EngagementWorkspaceComponent),
    children: [
      {
        path: ENGAGEMENT_ROUTES.OVERVIEW,
        loadComponent: () =>
          import(
            './engagement-workspace/overview/overview.component'
          ).then((m) => m.OverviewComponent),
      },
      {
        path: ENGAGEMENT_ROUTES.HANDOVER,
        loadComponent: () =>
          import('./engagement-workspace/handover/handover.component').then(
            (m) => m.HandoverComponent
          ),
      },
      {
        path: ENGAGEMENT_ROUTES.DOCUMENTS,
        loadComponent: () =>
          import(
            './engagement-workspace/documents/engagement-documents/engagement-documents.component'
          ).then((m) => m.EngagementDocumentsComponent),
      },
      {
        path: ENGAGEMENT_ROUTES.PEOPLE,
        loadComponent: () =>
          import('./engagement-workspace/people/people.component').then(
            (m) => m.PeopleComponent
          ),
      },
      {
        path: ENGAGEMENT_ROUTES.CHECKLIST,
        loadComponent: () =>
          import(
            './engagement-workspace/checklist/engagement-checklist.component'
          ).then((m) => m.EngagementChecklistComponent),
      },
      {
        path: ENGAGEMENT_ROUTES.KICKOFF,
        loadComponent: () =>
          import('./engagement-workspace/kickoff/kickoff.component').then(
            (m) => m.KickoffComponent
          ),
      },
      {
        path: ENGAGEMENT_ROUTES.RESOURCES,
        loadComponent: () =>
          import('./engagement-workspace/resources/resources.component').then(
            (m) => m.ResourcesComponent
          ),
      },
      {
        path: TECHNICAL_READINESS_ROUTES.BASE,
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
        path: ENGAGEMENT_ROUTES.ASSUMPTIONS,
        loadComponent: () =>
          import('./engagement-workspace/assumptions-and-decisions/components/assumptions/assumptions.component').then(
            (m) => m.AssumptionsComponent
          ),
      },
      {
        path: ENGAGEMENT_ROUTES.AUDITS,
        loadComponent: () =>
          import(
            './engagement-workspace/audits/engagement-audits/engagement-audits.component'
          ).then((m) => m.EngagementAuditsComponent),
      },
      {
        path: '',
        redirectTo: ENGAGEMENT_ROUTES.OVERVIEW,
        pathMatch: 'full',
      },
    ],
  },
];
