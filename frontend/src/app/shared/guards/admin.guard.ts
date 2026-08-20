import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@shared/services/auth';
import { APP_ROUTES } from '@shared/constants/app.constants';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn() && authService.isAdmin()) {
    return true;
  }

  if (!authService.isLoggedIn()) {
    router.navigate([APP_ROUTES.LOGIN]);
  } else {
    router.navigate([APP_ROUTES.ACCESS_DENIED]);
  }

  return false;
};
