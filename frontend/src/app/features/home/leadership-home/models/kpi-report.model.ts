export type PeriodPreset = 'AllTime' | 'LastMonth' | 'Last6Months' | 'LastYear' | 'Custom';

export interface KpiOptionDto {
  id: number;
  name: string;
}

export interface KpiReportFilterOptions {
  availablePms: KpiOptionDto[];
  availableEngagementTypes: KpiOptionDto[];
}

export interface KpiReportFilters {
  datePreset: PeriodPreset;
  customFrom: string | null;
  customTo: string | null;
  pmUserIds: number[];
  engagementTypeIds: number[];
}

export interface ReadinessTimeValue {
  averageDays: number | null;
  status: number;
}

export interface ReadinessTimeKpi {
  current: ReadinessTimeValue;
  prior: ReadinessTimeValue | null;
  absoluteChangeDays: number | null;
  hasPriorPeriodComparison: boolean;
}

export interface KpiAverageDaysValue {
  averageDays: number | null;
  status: number;
}

export interface AccessDelayKpi {
  current: KpiAverageDaysValue;
  prior: KpiAverageDaysValue | null;
  absoluteChangeDays: number | null;
  hasPriorPeriodComparison: boolean;
}

export interface KpiCountValue {
  count: number | null;
  status: number;
}

export interface EscalationCountKpi {
  current: KpiCountValue;
  prior: KpiCountValue | null;
  absoluteChange: number | null;
  hasPriorPeriodComparison: boolean;
}

export interface OnTimeKickoffValue {
  rate: number | null;
  numerator: number;
  denominator: number;
  status: number;
}

export interface OnTimeKickoffKpi {
  current: OnTimeKickoffValue;
  prior: OnTimeKickoffValue | null;
  absoluteChangeRate: number | null;
  hasPriorPeriodComparison: boolean;
}

export interface KpiReportResponse {
  periodFrom: string | null;
  periodTo: string | null;
  priorPeriodFrom: string | null;
  priorPeriodTo: string | null;
  relevantEngagementCount: number;
  readinessTime: ReadinessTimeKpi;
  accessDelay: AccessDelayKpi;
  escalationCount: EscalationCountKpi;
  onTimeKickoffRate: OnTimeKickoffKpi;
}
