export const ENGAGEMENT_CHECKLIST_MESSAGES = {
  LOAD_FAILED: 'Failed to load the engagement checklist. Please try again.'
} as const;

export const ENGAGEMENT_WORKSPACE_CONFIG = {
  DEFAULT_TABS: [
    { id: 'handover', label: 'Handover', route: 'handover', enabled: true },
    { id: 'documents', label: 'Documents', route: 'documents', enabled: true },
    { id: 'people', label: 'People', route: 'people', enabled: false },
    { id: 'checklist', label: 'Checklist', route: 'checklist', enabled: true },
    { id: 'kickoff', label: 'Kickoff', route: 'kickoff', enabled: true },
    { id: 'resourcing', label: 'Resourcing', route: 'resourcing', enabled: false },
    { id: 'technical-readiness', label: 'Technical Readiness', route: 'technical-readiness', enabled: true },
    { id: 'risks', label: 'Risks', route: 'risks', enabled: false },
    { id: 'reporting', label: 'Reporting', route: 'reporting', enabled: false }
  ]
} as const;
