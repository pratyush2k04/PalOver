import { Component, inject, OnInit } from '@angular/core';
import { AppContextService } from '@shared/services/app-context';
import { DashboardKpiCardComponent } from '@shared/components/dashboard-kpi-card/dashboard-kpi-card.component';
@Component({
  selector: 'app-pre-sales-home',
  standalone: true,
  imports: [DashboardKpiCardComponent],
  templateUrl: './pre-sales-home.html',
  styleUrl: './pre-sales-home.scss'
})
export class PreSalesHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;

  ngOnInit() {
    this.appContext.refreshContext();
  }
}
