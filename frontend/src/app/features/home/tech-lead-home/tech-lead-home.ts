import { Component, inject, OnInit } from '@angular/core';
import { AppContextService } from '@shared/services/app-context';
import { DashboardKpiCardComponent } from '@shared/components/dashboard-kpi-card/dashboard-kpi-card.component';
@Component({
  selector: 'app-tech-lead-home',
  standalone: true,
  imports: [DashboardKpiCardComponent],
  templateUrl: './tech-lead-home.html',
  styleUrl: './tech-lead-home.scss'
})
export class TechLeadHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;

  ngOnInit() {
    this.appContext.refreshContext();
  }
}
