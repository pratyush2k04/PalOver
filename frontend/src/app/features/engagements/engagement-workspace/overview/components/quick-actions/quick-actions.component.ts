import { Component, computed, inject } from '@angular/core';
import { AuthService } from '@shared/services/auth';
import { USER_ROLES } from '@shared/constants/app.constants';
import { ActionTileComponent } from '@common/components/action-tile/action-tile';

@Component({
  selector: 'app-quick-actions',
  standalone: true,
  imports: [ActionTileComponent],
  templateUrl: './quick-actions.component.html',
  styleUrl: './quick-actions.component.scss'
})
export class QuickActionsComponent {
  private authService = inject(AuthService);

  private readonly hasRole = (role: string) => this.authService.hasRole(role);

  readonly canUpdateChecklist = computed(
    () =>
      this.hasRole(USER_ROLES.PROJECT_MANAGEMENT) ||
      this.hasRole(USER_ROLES.TECH_LEAD) ||
      this.authService.isAdmin()
  );

  readonly canRequestResourcing = computed(() => this.hasRole(USER_ROLES.PROJECT_MANAGEMENT));

  readonly canAddDependency = computed(
    () =>
      this.hasRole(USER_ROLES.PROJECT_MANAGEMENT) ||
      this.hasRole(USER_ROLES.TECH_LEAD) ||
      this.hasRole(USER_ROLES.PRE_SALES) ||
      this.hasRole(USER_ROLES.LEADERSHIP)
  );

  readonly canAddCommitment = computed(() => this.hasRole(USER_ROLES.PRE_SALES));

  readonly canAddPerson = computed(() => this.hasRole(USER_ROLES.PRE_SALES));
}
