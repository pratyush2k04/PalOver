import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input
} from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';

import { LoaderService } from '@shared/services/loader.service';

@Component({
  selector: 'app-global-loader',
  standalone: true,
  templateUrl: './global-loader.html',
  styleUrl: './global-loader.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GlobalLoader {
  readonly loader = inject(LoaderService);
  private readonly router = inject(Router);
  readonly fullPage = input(false);

  readonly isWorkspaceRoute = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map((e) => e.urlAfterRedirects.includes('/workspace'))
    ),
    { initialValue: this.router.url.includes('/workspace') }
  );

  readonly isSubTabLoading = computed(() => {
    return this.isWorkspaceRoute() && !this.loader.isInitialWorkspaceLoad();
  });
}
