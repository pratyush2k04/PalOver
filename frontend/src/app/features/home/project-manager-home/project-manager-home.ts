import { Component, inject } from '@angular/core';
import { AppContextService } from '../../../services/app-context';
import { WelcomeBannerComponent } from '../../../shared/welcome-banner/welcome-banner';
import { RoleBadgeComponent } from '../../../shared/role-badge/role-badge';
import { WorkspacePlaceholderComponent } from '../../../shared/workspace-placeholder/workspace-placeholder';

@Component({
  selector: 'app-project-manager-home',
  standalone: true,
  imports: [WelcomeBannerComponent, RoleBadgeComponent, WorkspacePlaceholderComponent],
  templateUrl: './project-manager-home.html',
  styleUrl: './project-manager-home.scss'
})
export class ProjectManagerHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
