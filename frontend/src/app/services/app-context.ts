import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AppContext, HomeResponse } from '../models/app-context.model';

@Injectable({
  providedIn: 'root'
})
export class AppContextService {
  private http = inject(HttpClient);

  private contextSignal = signal<AppContext | null>(null);
  private initializedSignal = signal(false);

  readonly context = this.contextSignal.asReadonly();
  readonly initialized = this.initializedSignal.asReadonly();
  readonly user = computed(() => this.contextSignal()?.user ?? null);
  readonly workspace = computed(() => this.contextSignal()?.workspace ?? null);

  async initialize(): Promise<AppContext> {
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
