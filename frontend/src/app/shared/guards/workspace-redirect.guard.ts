import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AppContextService } from '../../services/app-context';

export const workspaceRedirectGuard: CanActivateFn = () => {
  const appContext = inject(AppContextService);
  const router = inject(Router);

  const workspace = appContext.workspace();
  if (workspace) {
    router.navigateByUrl(workspace.route);
  }

  return false;
};
