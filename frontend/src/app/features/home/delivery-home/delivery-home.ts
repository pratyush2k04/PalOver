import { Component, inject, OnInit, computed } from '@angular/core';
import { AppContextService } from '@shared/services/app-context';
import { DashboardKpiCardComponent } from '@shared/components/dashboard-kpi-card/dashboard-kpi-card.component';
import { DashboardEngagementsListComponent } from '@shared/components/dashboard-engagements-list/dashboard-engagements-list';
import { DashboardCriticalBlockersComponent } from '@shared/components/dashboard-critical-blockers/dashboard-critical-blockers';
import { PageHeaderComponent } from '@common/components/page-header/page-header';

import { ENGAGEMENT_ROLES } from '@shared/constants/app.constants';

@Component({
  selector: 'app-delivery-home',
  standalone: true,
  imports: [
    DashboardKpiCardComponent,
    DashboardEngagementsListComponent,
    DashboardCriticalBlockersComponent,
    PageHeaderComponent
  ],
  templateUrl: './delivery-home.html',
  styleUrl: './delivery-home.scss'
})
export class DeliveryHomeComponent implements OnInit {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;

  readonly pmEngagements = computed(() => {
    const engagements = this.workspace()?.upcomingEngagements ?? [];
    return engagements.filter((e) => !e.userRoles || e.userRoles.includes(ENGAGEMENT_ROLES.PROJECT_MANAGER));
  });

  readonly tlEngagements = computed(() => {
    const engagements = this.workspace()?.upcomingEngagements ?? [];
    return engagements.filter((e) => !e.userRoles || e.userRoles.includes(ENGAGEMENT_ROLES.TECH_LEAD));
  });

  ngOnInit() {
    this.appContext.refreshContext();
  }
}
