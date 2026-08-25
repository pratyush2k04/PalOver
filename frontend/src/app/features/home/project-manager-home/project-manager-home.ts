import { Component, inject, OnInit } from '@angular/core';
import { AppContextService } from '@shared/services/app-context';
import { DashboardKpiCardComponent } from '@shared/components/dashboard-kpi-card/dashboard-kpi-card.component';
import { DashboardEngagementsListComponent } from '@shared/components/dashboard-engagements-list/dashboard-engagements-list';
import { DashboardCriticalBlockersComponent } from '@shared/components/dashboard-critical-blockers/dashboard-critical-blockers';

@Component({
  selector: 'app-project-manager-home',
  standalone: true,
  imports: [DashboardKpiCardComponent, DashboardEngagementsListComponent, DashboardCriticalBlockersComponent],
  templateUrl: './project-manager-home.html',
  styleUrl: './project-manager-home.scss'
})
export class ProjectManagerHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;

  ngOnInit() {
    this.appContext.refreshContext();
  }
}
