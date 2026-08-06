import { Component, inject } from '@angular/core';
import { AppContextService } from '../../../services/app-context';
import { BannerComponent } from '../../../common/components/banner/banner';
import { BadgeComponent } from '../../../common/components/badge/badge';
import { WorkspaceCardComponent } from '../../../shared/workspace-card/workspace-card';

@Component({
  selector: 'app-tech-lead-home',
  standalone: true,
  imports: [BannerComponent, BadgeComponent, WorkspaceCardComponent],
  templateUrl: './tech-lead-home.html',
  styleUrl: './tech-lead-home.scss'
})
export class TechLeadHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
