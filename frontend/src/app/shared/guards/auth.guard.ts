import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@shared/services/auth';
import { AppContextService } from '@shared/services/app-context';

export const authGuard: CanActivateFn = async () => {
  const authService = inject(AuthService);
  const appContext = inject(AppContextService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  try {
    await appContext.initialize();
    return true;
  } catch {
    authService.logout();
    appContext.reset();
    router.navigate(['/login']);
    return false;
  }
};
