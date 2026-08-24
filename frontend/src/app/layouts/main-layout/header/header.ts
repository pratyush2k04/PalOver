import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AppContextService } from '@shared/services/app-context';
import { AuthService } from '@shared/services/auth';
import { APP_ROUTES } from '@shared/constants/app.constants';
import { AvatarComponent } from '@common/components/avatar/avatar';
import { NotificationsDropdownComponent } from '@shared/notifications-dropdown/notifications-dropdown';
import { GlobalSearchComponent } from '@shared/global-search/global-search';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [AvatarComponent, InlineSVGModule, NotificationsDropdownComponent, GlobalSearchComponent],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  private appContext = inject(AppContextService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private elementRef = inject(ElementRef);

  readonly user = this.appContext.user;
  readonly workspace = this.appContext.workspace;
  readonly isUserMenuOpen = signal(false);

  toggleUserMenu(): void {
    this.isUserMenuOpen.update(open => !open);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.isUserMenuOpen() && !this.elementRef.nativeElement.contains(event.target)) {
      this.isUserMenuOpen.set(false);
    }
  }

  onProfileClick(): void {
    this.isUserMenuOpen.set(false);
    this.router.navigate([APP_ROUTES.PROFILE]);
  }

  onLogOut(): void {
    this.isUserMenuOpen.set(false);
    this.authService.logout();
    this.appContext.reset();
    this.router.navigate([APP_ROUTES.LOGIN]);
  }
}
