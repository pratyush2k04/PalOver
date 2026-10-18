import { Component, inject, OnInit } from '@angular/core';
import { AppContextService } from '@shared/services/app-context';
import { DashboardKpiCardComponent } from '@shared/components/dashboard-kpi-card/dashboard-kpi-card.component';
import { DashboardEngagementsListComponent } from '@shared/components/dashboard-engagements-list/dashboard-engagements-list';
import { DashboardCriticalBlockersComponent } from '@shared/components/dashboard-critical-blockers/dashboard-critical-blockers';
import { PageHeaderComponent } from '@common/components/page-header/page-header';

@Component({
  selector: 'app-pre-sales-home',
  standalone: true,
  imports: [DashboardKpiCardComponent, DashboardEngagementsListComponent, DashboardCriticalBlockersComponent, PageHeaderComponent],
  templateUrl: './pre-sales-home.html',
  styleUrl: './pre-sales-home.scss'
})
export class PreSalesHomeComponent implements OnInit {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;

  ngOnInit() {
    this.appContext.refreshContext();
  }
}
