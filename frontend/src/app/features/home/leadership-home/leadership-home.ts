import { Component, inject } from '@angular/core';
import { AppContextService } from '../../../services/app-context';
import { BannerComponent } from '../../../common/components/banner/banner';
import { BadgeComponent } from '../../../common/components/badge/badge';
import { WorkspaceCardComponent } from '../../../shared/workspace-card/workspace-card';

@Component({
  selector: 'app-leadership-home',
  standalone: true,
  imports: [BannerComponent, BadgeComponent, WorkspaceCardComponent],
  templateUrl: './leadership-home.html',
  styleUrl: './leadership-home.scss'
})
export class LeadershipHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
