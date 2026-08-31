import { Component, computed, inject } from '@angular/core';
import { AuthService } from '@shared/services/auth';
import { EngagementService } from '../../../../services/engagement.service';
import { USER_ROLES, ENGAGEMENT_ROUTES } from '@shared/constants/app.constants';
import { ActionTileComponent } from '@common/components/action-tile/action-tile';

@Component({
  selector: 'app-quick-actions',
  standalone: true,
  imports: [ActionTileComponent],
  templateUrl: './quick-actions.component.html',
  styleUrl: './quick-actions.component.scss'
})
export class QuickActionsComponent {
  readonly ENGAGEMENT_ROUTES = ENGAGEMENT_ROUTES;

  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);

  private readonly hasRole = (role: string) => this.authService.hasRole(role);
  private readonly isSignedOff = computed(() => this.engagementService.isSignedOff());

  readonly canLogRisk = computed(() => !this.isSignedOff());

  readonly canUploadDocument = computed(() => !this.isSignedOff());

  readonly canUpdateChecklist = computed(
    () =>
      !this.isSignedOff() &&
      (this.hasRole(USER_ROLES.PROJECT_MANAGEMENT) ||
        this.hasRole(USER_ROLES.TECH_LEAD) ||
        this.authService.isAdmin())
  );

  readonly canRequestResourcing = computed(
    () => !this.isSignedOff() && this.hasRole(USER_ROLES.PROJECT_MANAGEMENT)
  );

  readonly canAddDependency = computed(
    () =>
      !this.isSignedOff() &&
      (this.hasRole(USER_ROLES.PROJECT_MANAGEMENT) ||
        this.hasRole(USER_ROLES.TECH_LEAD) ||
        this.hasRole(USER_ROLES.PRE_SALES) ||
        this.hasRole(USER_ROLES.LEADERSHIP))
  );

  readonly canAddCommitment = computed(
    () => !this.isSignedOff() && this.hasRole(USER_ROLES.PRE_SALES)
  );

  readonly canAddPerson = computed(
    () => !this.isSignedOff() && this.hasRole(USER_ROLES.PRE_SALES)
  );

  readonly canAddFeature = computed(
    () => !this.isSignedOff() && this.hasRole(USER_ROLES.TECH_LEAD)
  );
}
