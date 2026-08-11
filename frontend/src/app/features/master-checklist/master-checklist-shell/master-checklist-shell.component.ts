import { Component, inject, signal, computed } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { MASTER_CHECKLIST_ROUTES } from '../../../shared/constants/app.constants';
import { TabItem, TabsComponent } from '@common/components/tabs/tabs';

@Component({
  selector: 'app-master-checklist-shell',
  standalone: true,
  imports: [RouterOutlet, TabsComponent],
  templateUrl: './master-checklist-shell.component.html',
  styleUrl: './master-checklist-shell.component.scss'
})
export class MasterChecklistShellComponent {
  private router = inject(Router);

  readonly tabs = signal<TabItem[]>([
    { id: MASTER_CHECKLIST_ROUTES.DRAFT, label: 'Current Draft', route: MASTER_CHECKLIST_ROUTES.DRAFT, enabled: true },
    { id: MASTER_CHECKLIST_ROUTES.PUBLISHED, label: 'Published Checklist', route: MASTER_CHECKLIST_ROUTES.PUBLISHED, enabled: true },
    { id: MASTER_CHECKLIST_ROUTES.HISTORY, label: 'Version History', route: MASTER_CHECKLIST_ROUTES.HISTORY, enabled: true }
  ]);

  readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map(() => this.router.url)
    ),
    { initialValue: this.router.url }
  );

  readonly activeTabId = computed(
    () => this.tabs().find((tab) => this.currentUrl().includes(`/${tab.route}`))?.id ?? MASTER_CHECKLIST_ROUTES.DRAFT
  );
}
