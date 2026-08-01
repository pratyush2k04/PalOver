import { Component } from '@angular/core';

interface NavItem {
  label: string;
  icon: string;
  badge?: number | 'dot';
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  readonly navItems: NavItem[] = [
    { label: 'Home', icon: 'home' },
    { label: 'Engagements', icon: 'layers' },
    { label: 'My Handovers', icon: 'clipboard' },
    { label: 'Alerts', icon: 'bell', badge: 'dot' },
    { label: 'Profile', icon: 'user' }
  ];

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
}
