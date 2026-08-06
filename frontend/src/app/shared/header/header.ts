import { Component, inject } from '@angular/core';
import { AppContextService } from '../../services/app-context';
import { AvatarComponent } from '../../common/components/avatar/avatar';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [AvatarComponent],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
