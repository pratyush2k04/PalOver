import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { AuthService } from '@shared/services/auth';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { ACCESS_DENIED_ROUTES, AUTH_ROUTES } from '@shared/constants/app.constants';

export const engagementAccessGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const engagementService = inject(EngagementService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate([`/${AUTH_ROUTES.LOGIN}`]);
    return of(false);
  }

  const idParam = route.paramMap.get('id') || route.parent?.paramMap.get('id') || route.paramMap.get('engagementId');
  const engagementId = Number(idParam);

  if (!engagementId || isNaN(engagementId)) {
    router.navigate([`/${ACCESS_DENIED_ROUTES.BASE}`]);
    return of(false);
  }

  return engagementService.getEngagementInfo(engagementId).pipe(
    map(() => true),
    catchError(() => {
      router.navigate([`/${ACCESS_DENIED_ROUTES.BASE}`]);
      return of(false);
    })
  );
};
