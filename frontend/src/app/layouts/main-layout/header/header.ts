import { Component, inject } from '@angular/core';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AppContextService } from '../../../services/app-context';
import { AvatarComponent } from '@common/components/avatar/avatar';
import { NotificationsDropdownComponent } from '../../../shared/notifications-dropdown/notifications-dropdown';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [AvatarComponent, InlineSVGModule, NotificationsDropdownComponent],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  private appContext = inject(AppContextService);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
}
