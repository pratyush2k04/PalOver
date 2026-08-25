import { Component, inject } from '@angular/core';
import { Router, RouterOutlet, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { SidebarComponent } from './sidebar/sidebar';
import { HeaderComponent } from './header/header';
import { ToastComponent } from '@common/components/toast/toast';
import { GlobalLoader } from '@common/components/global-loader/global-loader';
import { LoaderService } from '@shared/services/loader.service';
import { LayoutService } from '@shared/services/layout.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, SidebarComponent, ToastComponent, GlobalLoader],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayoutComponent {
  private router = inject(Router);
  private loaderService = inject(LoaderService);
  readonly layoutService = inject(LayoutService);

  constructor() {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        this.loaderService.show();
        this.layoutService.closeSidebar();
      } else if (
        event instanceof NavigationEnd ||
        event instanceof NavigationCancel ||
        event instanceof NavigationError
      ) {
        this.loaderService.hide();
      }
    });
  }
}
