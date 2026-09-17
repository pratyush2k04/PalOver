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
  private readonly isPm = computed(() => this.engagementService.isWorkspacePm());
  private readonly isTechLead = computed(() => this.engagementService.isWorkspaceTechLead());

  readonly canLogRisk = computed(() => !this.isSignedOff());

  readonly canUploadDocument = computed(() => !this.isSignedOff());

  readonly canUpdateChecklist = computed(
    () =>
      !this.isSignedOff() &&
      (this.isPm() ||
        this.isTechLead() ||
        this.authService.isAdmin())
  );

  readonly canRequestResourcing = computed(
    () => !this.isSignedOff() && this.isPm()
  );

  readonly canAddDependency = computed(
    () =>
      !this.isSignedOff() &&
      (this.isPm() ||
        this.isTechLead() ||
        this.hasRole(USER_ROLES.PRE_SALES) ||
        this.engagementService.isWorkspaceLeadership() ||
        this.authService.isAdmin())
  );

  readonly canAddCommitment = computed(
    () => !this.isSignedOff() && this.hasRole(USER_ROLES.PRE_SALES)
  );

  readonly canAddPerson = computed(
    () => !this.isSignedOff() && this.hasRole(USER_ROLES.PRE_SALES)
  );

  readonly canAddFeature = computed(
    () => !this.isSignedOff() && this.isTechLead()
  );
}
