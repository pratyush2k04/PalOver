export const KPI_REPORT_API_ENDPOINTS = {
  REPORT: '/api/kpi-reports',
  FILTERS: '/api/kpi-reports/filters',
} as const;

export const KPI_REPORT_MESSAGES = {
  LOAD_ERROR: 'Unable to load the KPI report. Please try again.',
  LOAD_FILTERS_ERROR: 'Unable to load report filter options.',
  NO_ENGAGEMENTS: 'No engagements in this period.',
} as const;

export const KPI_REPORT_PERIOD_OPTIONS = [
  { value: 'AllTime', label: 'Any date' },
  { value: 'LastMonth', label: 'Last month' },
  { value: 'Last6Months', label: 'Last 6 months' },
  { value: 'LastYear', label: 'Last year' },
  { value: 'Custom', label: 'Custom range' },
] as const;

export const KPI_PERIOD_STATUS = {
  OK: 0,
  NO_ENGAGEMENTS_IN_PERIOD: 1,
} as const;
