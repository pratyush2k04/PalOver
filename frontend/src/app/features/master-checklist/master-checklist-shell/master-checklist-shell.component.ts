import { Component, signal } from '@angular/core';

import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MASTER_CHECKLIST_ROUTES } from '../../../constants/app.constants';

interface WorkspaceTab {
  label: string;
  route: string;
  enabled: boolean;
}

@Component({
  selector: 'app-master-checklist-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './master-checklist-shell.component.html',
  styleUrl: './master-checklist-shell.component.scss'
})
export class MasterChecklistShellComponent {
  readonly tabs = signal<WorkspaceTab[]>([
    { label: 'Current Draft', route: `./${MASTER_CHECKLIST_ROUTES.DRAFT}`, enabled: true },
    { label: 'Published Checklist', route: `./${MASTER_CHECKLIST_ROUTES.PUBLISHED}`, enabled: true },
    { label: 'Version History', route: `./${MASTER_CHECKLIST_ROUTES.HISTORY}`, enabled: true }
  ]);

  activeTabIndex = signal<number>(0);

  onTabKeydown(event: KeyboardEvent) {
    const enabledIndexes = this.tabs()
      .map((tab, index) => ({ tab, index }))
      .filter(({ tab }) => tab.enabled)
      .map(({ index }) => index);

    if (enabledIndexes.length === 0) {
      return;
    }

    const currentPos = enabledIndexes.indexOf(this.activeTabIndex());

    if (event.key === 'ArrowRight' || event.key === 'ArrowDown') {
      event.preventDefault();
      const nextPos = (currentPos + 1) % enabledIndexes.length;
      this.focusTab(enabledIndexes[nextPos]);
    } else if (event.key === 'ArrowLeft' || event.key === 'ArrowUp') {
      event.preventDefault();
      const prevPos = (currentPos - 1 + enabledIndexes.length) % enabledIndexes.length;
      this.focusTab(enabledIndexes[prevPos]);
    }
  }

  private focusTab(index: number) {
    this.activeTabIndex.set(index);
    const tabElements = document.querySelectorAll<HTMLElement>('.tab-item');
    tabElements[index]?.focus();
    tabElements[index]?.click();
  }
}
