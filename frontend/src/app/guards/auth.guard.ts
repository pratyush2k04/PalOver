import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';
import { AppContextService } from '../services/app-context';
import { APP_ROUTES } from '@shared/constants/app.constants';

export const authGuard: CanActivateFn = async () => {
  const authService = inject(AuthService);
  const appContext = inject(AppContextService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate([APP_ROUTES.LOGIN]);
    return false;
  }

  try {
    await appContext.initialize();
    return true;
  } catch {
    authService.logout();
    appContext.reset();
    router.navigate([APP_ROUTES.LOGIN]);
    return false;
  }
};
