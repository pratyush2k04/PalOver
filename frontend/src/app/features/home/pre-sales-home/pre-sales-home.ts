import { Component, inject } from '@angular/core';
import { AppContextService } from '@shared/services/app-context';
import { BannerComponent } from '@common/components/banner/banner';
import { BadgeComponent } from '@common/components/badge/badge';
import { WorkspaceCardComponent } from '@shared/workspace-card/workspace-card';

@Component({
  selector: 'app-pre-sales-home',
  standalone: true,
  imports: [BannerComponent, BadgeComponent, WorkspaceCardComponent],
  templateUrl: './pre-sales-home.html',
  styleUrl: './pre-sales-home.scss'
})
export class PreSalesHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
