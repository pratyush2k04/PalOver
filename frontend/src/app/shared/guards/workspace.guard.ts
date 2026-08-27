import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AppContextService } from '@shared/services/app-context';

export const workspaceGuard: CanActivateFn = (route) => {
  const appContext = inject(AppContextService);
  const router = inject(Router);

  const workspace = appContext.workspace();
  const requestedPath = `/${route.routeConfig?.path ?? ''}`;

  if (workspace && workspace.route !== requestedPath) {
    router.navigateByUrl(workspace.route);
    return false;
  }

  return true;
};
