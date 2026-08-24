import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
import { KPI_PERIOD_STATUS, KPI_REPORT_MESSAGES, KPI_REPORT_PERIOD_OPTIONS } from '@shared/constants/app.constants';
import { KpiReportService } from './services/kpi-report.service';
import { KpiOptionDto, KpiReportResponse, PeriodPreset } from './models/kpi-report.model';

@Component({
  selector: 'app-leadership-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    PageHeaderComponent,
    BadgeComponent,
    LoadingSpinnerComponent,
    AlertComponent,
    EmptyStateComponent,
    StatusBadgeComponent,
    SelectComponent,
    InputComponent,
    MultiSelectListComponent
  ],
  templateUrl: './leadership-home.html',
  styleUrl: './leadership-home.scss'
})
export class LeadershipHomeComponent implements OnInit {
  private appContext = inject(AppContextService);
  private kpiReportService = inject(KpiReportService);

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

  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  report = signal<KpiReportResponse | null>(null);

  readonly showCustomRange = computed(() => this.datePreset === 'Custom');

  readonly isEmpty = computed(() => {
    const r = this.report();
    return r != null && r.relevantEngagementCount === 0;
  });

  ngOnInit(): void {
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
