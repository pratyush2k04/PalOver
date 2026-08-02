import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AppContextService } from '../../services/app-context';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  private appContext = inject(AppContextService);
  private authService = inject(AuthService);
  private router = inject(Router);

  readonly workspace = this.appContext.workspace;

  private readonly glyphs: Record<string, string> = {
    home: '⌂',
    layers: '▤',
    clipboard: '▥',
    bell: '🔔',
    user: '◍',
    'sign-out': '⇥'
  };

  iconGlyph(icon: string): string {
    return this.glyphs[icon] ?? '•';
  }

  onSignOut(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
