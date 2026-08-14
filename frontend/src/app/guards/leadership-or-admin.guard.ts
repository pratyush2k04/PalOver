import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';
import { USER_ROLES } from '@shared/constants/app.constants';

export const leadershipOrAdminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn() && (authService.hasRole(USER_ROLES.LEADERSHIP) || authService.isAdmin())) {
    return true;
  }

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
  } else {
    router.navigate(['/access-denied']);
  }

  return false;
};
