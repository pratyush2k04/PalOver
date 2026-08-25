import { Component, inject } from '@angular/core';
import { AppContextService } from '@shared/services/app-context';
import { PageHeaderComponent } from '@common/components/page-header/page-header';

@Component({
  selector: 'app-admin-home',
  standalone: true,
  imports: [
    PageHeaderComponent
  ],
  templateUrl: './admin-home.html',
  styleUrl: './admin-home.scss'
})
export class AdminHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
