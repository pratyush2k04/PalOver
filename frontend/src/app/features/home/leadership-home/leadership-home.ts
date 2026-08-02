import { Component, inject } from '@angular/core';
import { AppContextService } from '../../../services/app-context';
import { WelcomeBannerComponent } from '../../../shared/welcome-banner/welcome-banner';
import { RoleBadgeComponent } from '../../../shared/role-badge/role-badge';
import { WorkspacePlaceholderComponent } from '../../../shared/workspace-placeholder/workspace-placeholder';

@Component({
  selector: 'app-leadership-home',
  standalone: true,
  imports: [WelcomeBannerComponent, RoleBadgeComponent, WorkspacePlaceholderComponent],
  templateUrl: './leadership-home.html',
  styleUrl: './leadership-home.scss'
})
export class LeadershipHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
