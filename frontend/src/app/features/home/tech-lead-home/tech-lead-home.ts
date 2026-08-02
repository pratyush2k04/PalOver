import { Component, inject } from '@angular/core';
import { AppContextService } from '../../../services/app-context';
import { WelcomeBannerComponent } from '../../../shared/welcome-banner/welcome-banner';
import { RoleBadgeComponent } from '../../../shared/role-badge/role-badge';
import { WorkspacePlaceholderComponent } from '../../../shared/workspace-placeholder/workspace-placeholder';

@Component({
  selector: 'app-tech-lead-home',
  standalone: true,
  imports: [WelcomeBannerComponent, RoleBadgeComponent, WorkspacePlaceholderComponent],
  templateUrl: './tech-lead-home.html',
  styleUrl: './tech-lead-home.scss'
})
export class TechLeadHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
