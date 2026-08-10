import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../services/auth';
import { EngagementDetailComponent } from './components/engagement-detail/engagement-detail';

@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [CommonModule, EngagementDetailComponent],
  templateUrl: './overview.component.html',
  styleUrl: './overview.component.scss'
})
export class OverviewComponent {
  private authService = inject(AuthService);

  readonly isLeadership = computed(() => this.authService.hasRole('Leadership'));
}
