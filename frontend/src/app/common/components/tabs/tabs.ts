import { Component, ElementRef, inject, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

export interface TabItem {
  id: string;
  label: string;
  route?: string;
  enabled?: boolean;
}

@Component({
  selector: 'app-tabs',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './tabs.html',
  styleUrl: './tabs.scss'
})
export class TabsComponent {
  private readonly elementRef: ElementRef<HTMLElement> = inject(ElementRef);

  readonly tabs = input.required<TabItem[]>();
  readonly activeId = input<string | null>(null);

  readonly tabChange = output<string>();

  isEnabled(tab: TabItem): boolean {
    return tab.enabled !== false;
  }

  isActive(tab: TabItem): boolean {
    return tab.id === this.activeId();
  }

  select(tab: TabItem): void {
    if (!this.isEnabled(tab)) {
      return;
    }
    this.tabChange.emit(tab.id);
  }

  onKeydown(event: KeyboardEvent): void {
    const enabledTabs = this.tabs().filter((tab) => this.isEnabled(tab));
    if (enabledTabs.length === 0) {
      return;
    }

    const currentPos = enabledTabs.findIndex((tab) => this.isActive(tab));

    if (event.key === 'ArrowRight' || event.key === 'ArrowDown') {
      event.preventDefault();
      const next = enabledTabs[(currentPos + 1) % enabledTabs.length];
      this.focus(next);
    } else if (event.key === 'ArrowLeft' || event.key === 'ArrowUp') {
      event.preventDefault();
      const prevPos = (currentPos - 1 + enabledTabs.length) % enabledTabs.length;
      this.focus(enabledTabs[prevPos]);
    }
  }

  private focus(tab: TabItem): void {
    const element = this.elementRef.nativeElement.querySelector<HTMLElement>(`[data-tab-id="${tab.id}"]`);
    element?.focus();
    element?.click();
  }
}
