import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth';

interface NavTab {
  label: string;
  path: string;
  adminOnly: boolean;
}

const ADMIN_NAV_TABS: NavTab[] = [
  { label: 'Dashboard', path: '/dashboard', adminOnly: false },
  { label: 'Master Checklist', path: '/checklist/draft', adminOnly: true },
  { label: 'Version History', path: '/checklist/history', adminOnly: true },
];

const NON_ADMIN_NAV_TABS: NavTab[] = [
  { label: 'Dashboard', path: '/dashboard', adminOnly: false },
  { label: 'Checklist', path: '/checklist/published', adminOnly: false },
];

@Component({
  selector: 'app-top-nav',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './top-nav.html',
  styleUrl: './top-nav.scss',
})
export class TopNavComponent {
  private authService = inject(AuthService);

  readonly visibleTabs = computed(() => {
    const isAdmin = this.authService.currentUser()?.isAdmin ?? false;
    return isAdmin ? ADMIN_NAV_TABS : NON_ADMIN_NAV_TABS;
  });
}
