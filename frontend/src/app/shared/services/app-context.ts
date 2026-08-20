import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AppContext, HomeResponse } from '@shared/models/app-context.model';

@Injectable({
  providedIn: 'root'
})
export class AppContextService {
  private http = inject(HttpClient);

  private contextSignal = signal<AppContext | null>(null);
  private initializedSignal = signal(false);
  private initializationPromise: Promise<AppContext> | null = null;

  readonly context = this.contextSignal.asReadonly();
  readonly initialized = this.initializedSignal.asReadonly();
  readonly user = computed(() => this.contextSignal()?.user ?? null);
  readonly workspace = computed(() => this.contextSignal()?.workspace ?? null);

  initialize(): Promise<AppContext> {
    if (this.contextSignal()) {
      return Promise.resolve(this.contextSignal()!);
    }

    if (!this.initializationPromise) {
      this.initializationPromise = this.fetchContext();
    }

    return this.initializationPromise;
  }

  reset(): void {
    this.contextSignal.set(null);
    this.initializedSignal.set(false);
    this.initializationPromise = null;
  }

  private async fetchContext(): Promise<AppContext> {
    const response = await firstValueFrom(this.http.get<HomeResponse>('/api/home'));

    const context: AppContext = {
      user: response.user,
      workspace: response.home
    };

    this.contextSignal.set(context);
    this.initializedSignal.set(true);

    return context;
  }
}
