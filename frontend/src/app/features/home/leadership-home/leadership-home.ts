import { Component, ElementRef, HostListener, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AppContextService } from '@shared/services/app-context';
import { PageHeaderComponent } from '@common/components/page-header/page-header';
import { BadgeComponent } from '@common/components/badge/badge';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { InputComponent } from '@common/components/form/input/input';
import { MultiSelectListComponent } from '@common/components/multi-select-list/multi-select-list';
import { ButtonComponent } from '@common/components/button/button';
import { DashboardKpiCardComponent } from '@shared/components/dashboard-kpi-card/dashboard-kpi-card.component';
import { DashboardEngagementsListComponent } from '@shared/components/dashboard-engagements-list/dashboard-engagements-list';
import { DashboardCriticalBlockersComponent } from '@shared/components/dashboard-critical-blockers/dashboard-critical-blockers';
import { downloadCsv } from '@shared/utils/csv.util';
import { KPI_PERIOD_STATUS, KPI_REPORT_MESSAGES, KPI_REPORT_PERIOD_OPTIONS } from '@shared/constants/app.constants';
import { KpiReportService } from './services/kpi-report.service';
import { KpiOptionDto, KpiReportResponse, PeriodPreset } from './models/kpi-report.model';

@Component({
  selector: 'app-leadership-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InlineSVGModule,
    DashboardKpiCardComponent,
    DashboardEngagementsListComponent,
    DashboardCriticalBlockersComponent
  ],
  templateUrl: './leadership-home.html',
  styleUrl: './leadership-home.scss'
})
export class LeadershipHomeComponent implements OnInit {
  private appContext = inject(AppContextService);
  private kpiReportService = inject(KpiReportService);
  private elementRef = inject(ElementRef);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;

  readonly periodOptions: SelectOption[] = KPI_REPORT_PERIOD_OPTIONS.map((o) => ({ value: o.value, label: o.label }));

  datePreset: PeriodPreset = 'AllTime';
  customFrom: string | null = null;
  customTo: string | null = null;
  selectedPmIds: number[] = [];
  selectedEngagementTypeIds: number[] = [];

  pmOptions = signal<SelectOption[]>([]);
  engagementTypeOptions = signal<SelectOption[]>([]);

  isPmPickerOpen = signal(false);
  isEngagementTypePickerOpen = signal(false);

  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  report = signal<KpiReportResponse | null>(null);

  readonly showCustomRange = computed(() => this.datePreset === 'Custom');

  readonly isEmpty = computed(() => {
    const r = this.report();
    return r != null && r.relevantEngagementCount === 0;
  });

  readonly pmFilterSummary = computed(() => this.buildFilterSummary(this.selectedPmIds, this.pmOptions(), 'PM'));
  readonly engagementTypeFilterSummary = computed(() =>
    this.buildFilterSummary(this.selectedEngagementTypeIds, this.engagementTypeOptions(), 'type')
  );

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isPmPickerOpen.set(false);
      this.isEngagementTypePickerOpen.set(false);
    }
  }

  ngOnInit(): void {
    this.appContext.refreshContext();
    this.loadFilterOptions();
    this.loadReport();
  }

  onPeriodPresetChange(): void {
    if (this.datePreset !== 'Custom') {
      this.customFrom = null;
      this.customTo = null;
      this.loadReport();
    }
  }

  onCustomDateChange(): void {
    if (this.customFrom && this.customTo) {
      this.loadReport();
    }
  }

  togglePmPicker(): void {
    this.isEngagementTypePickerOpen.set(false);
    this.isPmPickerOpen.update((open) => !open);
  }

  toggleEngagementTypePicker(): void {
    this.isPmPickerOpen.set(false);
    this.isEngagementTypePickerOpen.update((open) => !open);
  }

  onPmSelectionChange(values: unknown[]): void {
    this.selectedPmIds = values as number[];
    this.loadReport();
  }

  onEngagementTypeSelectionChange(values: unknown[]): void {
    this.selectedEngagementTypeIds = values as number[];
    this.loadReport();
  }

  isNoData(status: number): boolean {
    return status === KPI_PERIOD_STATUS.NO_ENGAGEMENTS_IN_PERIOD;
  }

  exportCsv(): void {
    const r = this.report();
    if (!r) {
      return;
    }

    const header = ['Metric', 'Current Value', 'Prior Value', 'Change'];
    const rows: string[][] = [
      [
        'Readiness Time (days)',
        this.isNoData(r.readinessTime.current.status) ? 'No data' : String(r.readinessTime.current.averageDays ?? ''),
        r.readinessTime.prior && !this.isNoData(r.readinessTime.prior.status)
          ? String(r.readinessTime.prior.averageDays ?? '')
          : '',
        r.readinessTime.hasPriorPeriodComparison && r.readinessTime.absoluteChangeDays !== null
          ? String(r.readinessTime.absoluteChangeDays)
          : ''
      ],
      [
        'Access Delay (days)',
        this.isNoData(r.accessDelay.current.status) ? 'No data' : String(r.accessDelay.current.averageDays ?? ''),
        r.accessDelay.prior && !this.isNoData(r.accessDelay.prior.status) ? String(r.accessDelay.prior.averageDays ?? '') : '',
        r.accessDelay.hasPriorPeriodComparison && r.accessDelay.absoluteChangeDays !== null
          ? String(r.accessDelay.absoluteChangeDays)
          : ''
      ],
      [
        'Escalation Count',
        String(r.escalationCount.current.count ?? ''),
        r.escalationCount.prior ? String(r.escalationCount.prior.count ?? '') : '',
        r.escalationCount.hasPriorPeriodComparison && r.escalationCount.absoluteChange !== null
          ? String(r.escalationCount.absoluteChange)
          : ''
      ],
      [
        'On-Time Kickoff Rate (%)',
        this.isNoData(r.onTimeKickoffRate.current.status) ? 'No data' : String((r.onTimeKickoffRate.current.rate ?? 0) * 100),
        r.onTimeKickoffRate.prior && !this.isNoData(r.onTimeKickoffRate.prior.status)
          ? String((r.onTimeKickoffRate.prior.rate ?? 0) * 100)
          : '',
        r.onTimeKickoffRate.hasPriorPeriodComparison && r.onTimeKickoffRate.absoluteChangeRate !== null
          ? String(r.onTimeKickoffRate.absoluteChangeRate * 100)
          : ''
      ]
    ];

    downloadCsv(rows, header, 'leadership-kpi-report');
  }

  private buildFilterSummary(selectedIds: number[], options: SelectOption[], noun: string): string {
    if (selectedIds.length === 0) {
      return `All ${noun}s`;
    }
    if (selectedIds.length === 1) {
      return options.find((o) => o.value === selectedIds[0])?.label ?? `1 ${noun}`;
    }
    return `${selectedIds.length} ${noun}s selected`;
  }

  private loadFilterOptions(): void {
    this.kpiReportService.getFilterOptions().subscribe({
      next: (options) => {
        this.pmOptions.set(options.availablePms.map((pm: KpiOptionDto) => ({ value: pm.id, label: pm.name })));
        this.engagementTypeOptions.set(
          options.availableEngagementTypes.map((t: KpiOptionDto) => ({ value: t.id, label: t.name }))
        );
      },
      error: () => this.errorMessage.set(KPI_REPORT_MESSAGES.LOAD_FILTERS_ERROR)
    });
  }

  private loadReport(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.kpiReportService
      .getReport({
        datePreset: this.datePreset,
        customFrom: this.customFrom,
        customTo: this.customTo,
        pmUserIds: this.selectedPmIds,
        engagementTypeIds: this.selectedEngagementTypeIds
      })
      .subscribe({
        next: (data) => {
          this.report.set(data);
          this.isLoading.set(false);
        },
        error: () => {
          this.errorMessage.set(KPI_REPORT_MESSAGES.LOAD_ERROR);
          this.isLoading.set(false);
        }
      });
  }
}
