export const ENGAGEMENT_CHECKLIST_MESSAGES = {
  LOAD_FAILED: 'Failed to load the engagement checklist. Please try again.'
} as const;

export const ENGAGEMENT_WORKSPACE_CONFIG = {
  DEFAULT_TABS: [
    { label: 'Handover', route: 'handover', enabled: true },
    { label: 'Documents', route: 'documents', enabled: true },
    { label: 'People', route: 'people', enabled: false },
    { label: 'Checklist', route: 'checklist', enabled: true },
    { label: 'Kickoff', route: 'kickoff', enabled: true },
    { label: 'Resourcing', route: 'resourcing', enabled: false },
    { label: 'Technical Readiness', route: 'technical-readiness', enabled: true },
    { label: 'Risks', route: 'risks', enabled: false },
    { label: 'Reporting', route: 'reporting', enabled: false }
  ]
} as const;
