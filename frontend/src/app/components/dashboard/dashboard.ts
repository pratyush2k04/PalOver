import { Component, OnInit, computed, inject } from '@angular/core';
import { AuthService } from '../../services/auth';
import { DashboardService } from '../../features/dashboard/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class DashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private dashboardService = inject(DashboardService);

  readonly loading = this.dashboardService.loading;
  readonly error = this.dashboardService.error;
  readonly summary = this.dashboardService.summary;

  readonly userFullName = computed(() => this.authService.currentUser()?.fullName || 'there');

  ngOnInit(): void {
    this.dashboardService.loadSummary();
  }
}
