import { SelectOption } from '@common/components/form/select/select';

export const RESOURCE_REQUEST_CONSTANTS = {
  // API Endpoint Paths
  API_ENGAGEMENT_RESOURCE_REQUESTS: (engagementId: number) =>
    `/api/engagements/${engagementId}/resource-requests`,

  // UI Strings
  PAGE_TITLE: 'Resource Requests',
  BTN_RAISE_REQUEST: '+ Raise request',
  MODAL_TITLE: 'Raise resource request',

  LABEL_STANDARD_ROLE: 'Standard role',
  LABEL_SKILLS: 'Skills',
  LABEL_NUMBER_OF_PEOPLE: 'Number of people',
  LABEL_REQUIRED_BY: 'Required by',
  LABEL_JOB_DESCRIPTION: 'Job description',

  BTN_CANCEL: 'Cancel',
  BTN_RAISE: 'Raise',

  // Validation Limits & Messages
  MIN_JOB_DESCRIPTION_LENGTH: 15,
  MAX_JOB_DESCRIPTION_LENGTH: 500,

  MSG_REQUIRED_BEFORE_BILLING: 'Resources are needed on or before the billing date.',
  MSG_REQUIRED_DATE_PAST: 'Required-by date cannot be in the past.',
  MSG_SKILL_REQUIRED: 'At least one skill must be selected.',
  MSG_PEOPLE_NEEDED_POSITIVE: 'Number of people must be greater than zero.',
  MSG_JOB_DESC_LENGTH: 'Job description must be between 15 and 500 characters.',
  MSG_SUCCESS_RAISED: 'Resource request raised successfully.',

  EMPTY_STATE_TITLE: 'No resource requests found',
  EMPTY_STATE_SUBTITLE: 'Structured resource requests raised for this engagement will appear here.',
} as const;

export const STANDARD_ROLE_OPTIONS: readonly SelectOption[] = [
  { value: 1, label: 'Senior Developer' },
  { value: 2, label: 'Developer' },
  { value: 3, label: 'QA Engineer' },
  { value: 4, label: 'Solution Architect' },
  { value: 5, label: 'Business Analyst' },
] as const;

export const SKILL_OPTIONS: readonly SelectOption[] = [
  { value: 1, label: 'React / Frontend' },
  { value: 2, label: 'Angular / Frontend' },
  { value: 3, label: 'Node.js / Backend' },
  { value: 4, label: '.NET / Backend' },
  { value: 5, label: 'Data Engineering' },
  { value: 6, label: 'QA Automation' },
  { value: 7, label: 'DevOps / Cloud' },
  { value: 8, label: 'Business Analysis' },
] as const;
