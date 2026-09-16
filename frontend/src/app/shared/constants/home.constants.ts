import { USER_ROLES } from './users.constants';
import { HOME_ROUTES_CONFIG } from './routes.constants';

export const HOME_MESSAGES = {
  WELCOME: 'Welcome to your workspace',
} as const;

export const DASHBOARD_CONSTANTS = {
  FALLBACK_TASK: 'TASK',
  TIME: {
    JUST_NOW: 'Just now',
    MINUTES_AGO: 'm ago',
    HOURS_AGO: 'h ago',
    DAYS_AGO: 'd ago'
  }
} as const;

export const ROLE_HOME_ROUTES: Record<string, string> = {
  [USER_ROLES.DELIVERY]: `/${HOME_ROUTES_CONFIG.ENGAGEMENT_DASHBOARD}`,
  [USER_ROLES.PROJECT_MANAGEMENT]: `/${HOME_ROUTES_CONFIG.ENGAGEMENT_DASHBOARD}`,
  [USER_ROLES.PRE_SALES]: `/${HOME_ROUTES_CONFIG.MY_HANDOVERS}`,
  [USER_ROLES.TECH_LEAD]: `/${HOME_ROUTES_CONFIG.TECHNICAL_READINESS}`,
  [USER_ROLES.LEADERSHIP]: `/${HOME_ROUTES_CONFIG.PORTFOLIO_DASHBOARD}`,
};

export const ROLE_HOME_TITLES: Record<string, string> = {
  [USER_ROLES.DELIVERY]: 'Delivery Workspace',
  [USER_ROLES.PROJECT_MANAGEMENT]: 'My Engagements',
  [USER_ROLES.PRE_SALES]: 'My Handovers',
  [USER_ROLES.TECH_LEAD]: 'Technical Readiness',
  [USER_ROLES.LEADERSHIP]: 'Portfolio Dashboard',
};

export const ROLE_EMPTY_STATES: Record<string, string> = {
  [USER_ROLES.DELIVERY]: 'You have no active engagements yet.',
  [USER_ROLES.PROJECT_MANAGEMENT]: 'You have no engagements yet.',
  [USER_ROLES.PRE_SALES]: 'You have no handovers yet.',
  [USER_ROLES.TECH_LEAD]: 'You have no technical readiness tasks yet.',
  [USER_ROLES.LEADERSHIP]: 'No portfolio items available.',
};

export const DASHBOARD_CARD_TITLES: Record<string, string> = {
  engagement: 'My Engagements',
  'draft-handover': 'Draft Handovers',
  'complete-handover': 'Completed Handovers',
  checklist: 'Checklist Actions',
  blocker: 'Open Blockers',
  'tech-queue': 'My Tech Queue',
  'access-item': 'Open Access Items',
  escalation: 'My Escalations',
};

