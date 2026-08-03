import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: number | 'dot';
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  readonly navItems: NavItem[] = [
    { label: 'Home', icon: 'home', route: '/dashboard' },
    { label: 'Engagements', icon: 'layers', route: '/engagements' },
    { label: 'My Engagements', icon: 'briefcase', route: '/my-engagements' },
    { label: 'My Handovers', icon: 'clipboard', route: '/dashboard' },
    { label: 'Alerts', icon: 'bell', route: '/dashboard', badge: 'dot' },
    { label: 'Profile', icon: 'user', route: '/dashboard' }
  ];

  private readonly glyphs: Record<string, string> = {
    home: '⌂',
    layers: '▤',
    briefcase: '💼',
    clipboard: '▥',
    bell: '🔔',
    user: '◍',
    'sign-out': '⇥'
  };

  iconGlyph(icon: string): string {
    return this.glyphs[icon] ?? '•';
  }
}
