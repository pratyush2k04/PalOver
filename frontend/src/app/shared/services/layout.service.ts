import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  readonly isMobileSidebarOpen = signal(false);

  toggleSidebar(): void {
    this.isMobileSidebarOpen.update((open) => !open);
  }

  openSidebar(): void {
    this.isMobileSidebarOpen.set(true);
  }

  closeSidebar(): void {
    this.isMobileSidebarOpen.set(false);
  }
}
