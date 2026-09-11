import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AppContext, HomeResponse, WorkspaceMetadata, DashboardCard } from '@shared/models/app-context.model';
import {
  ROLE_HOME_ROUTES,
  ROLE_HOME_TITLES,
  ROLE_EMPTY_STATES,
  DASHBOARD_CARD_TITLES,
  USER_ROLES,
  APP_ROUTES
} from '@shared/constants/app.constants';

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

  readonly homeRoute = computed(() => {
    const role = this.user()?.role;
    return role ? (ROLE_HOME_ROUTES[role] ?? APP_ROUTES.DASHBOARD) : APP_ROUTES.LOGIN;
  });

  readonly pageTitle = computed(() => {
    const role = this.user()?.role;
    return role ? (ROLE_HOME_TITLES[role] ?? '') : '';
  });

  readonly welcomeMessage = computed(() => {
    const u = this.user();
    return u ? `Welcome, ${u.fullName} - ${u.role}` : '';
  });

  readonly emptyState = computed(() => {
    const role = this.user()?.role;
    return role ? (ROLE_EMPTY_STATES[role] ?? '') : '';
  });

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

  async refreshContext(): Promise<AppContext> {
    this.initializationPromise = this.fetchContext();
    return this.initializationPromise;
  }

  private async fetchContext(): Promise<AppContext> {
    const response = await firstValueFrom(this.http.get<HomeResponse>('/api/home'));

    const user = response.user;
    const raw = response.home;

    const route = user?.role ? (ROLE_HOME_ROUTES[user.role] ?? APP_ROUTES.DASHBOARD) : APP_ROUTES.LOGIN;
    const title = user?.role ? (ROLE_HOME_TITLES[user.role] ?? '') : '';
    const welcomeMessage = user ? `Welcome, ${user.fullName} - ${user.role}` : '';
    const emptyState = user?.role ? (ROLE_EMPTY_STATES[user.role] ?? '') : '';

    const cards: DashboardCard[] = raw.cards.map((card) => {
      const cardTitle =
        card.type === 'engagement' && user.role === USER_ROLES.PRE_SALES
          ? 'My Handovers'
          : DASHBOARD_CARD_TITLES[card.type] ?? card.type;

      return {
        type: card.type,
        count: card.count,
        title: cardTitle
      };
    });

    const workspace: WorkspaceMetadata = {
      route,
      title,
      welcomeMessage,
      emptyState,
      cards,
      upcomingEngagements: raw.upcomingEngagements,
      totalUpcomingEngagements: raw.totalUpcomingEngagements,
      criticalBlockers: raw.criticalBlockers,
      totalCriticalBlockers: raw.totalCriticalBlockers
    };

    const context: AppContext = {
      user,
      workspace,
      rawDashboard: raw
    };

    this.contextSignal.set(context);
    this.initializedSignal.set(true);

    return context;
  }
}
