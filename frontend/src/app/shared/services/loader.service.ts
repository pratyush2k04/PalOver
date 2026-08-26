import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {
  readonly isLoading = signal<boolean>(false);
  readonly isInitialWorkspaceLoad = signal<boolean>(false);

  show(): void {
    this.isLoading.set(true);
  }

  hide(): void {
    this.isLoading.set(false);
  }

  setLoading(isLoading: boolean): void {
    this.isLoading.set(isLoading);
  }

  setInitialWorkspaceLoad(isInitial: boolean): void {
    this.isInitialWorkspaceLoad.set(isInitial);
  }
}
