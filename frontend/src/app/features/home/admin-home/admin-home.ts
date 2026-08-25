import { Component, inject } from '@angular/core';
import { AppContextService } from '@shared/services/app-context';
@Component({
  selector: 'app-admin-home',
  standalone: true,
  imports: [],
  templateUrl: './admin-home.html',
  styleUrl: './admin-home.scss'
})
export class AdminHomeComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
