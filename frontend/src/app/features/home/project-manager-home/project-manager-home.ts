import { Component, inject } from '@angular/core';
import { AppContextService } from '../../../services/app-context';
import { BannerComponent } from '@common/components/banner/banner';
import { BadgeComponent } from '@common/components/badge/badge';
import { WorkspaceCardComponent } from '@shared/workspace-card/workspace-card';

@Component({
  selector: 'app-project-manager-home',
  standalone: true,
  imports: [BannerComponent, BadgeComponent, WorkspaceCardComponent],
  templateUrl: './project-manager-home.html',
  styleUrl: './project-manager-home.scss'
})
export class ProjectManagerHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
