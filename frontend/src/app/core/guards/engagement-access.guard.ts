import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { AuthService } from 'src/app/services/auth';
import { HandoverService } from '../../features/engagements/services/handover.service';

export const engagementAccessGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const handoverService = inject(HandoverService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return of(false);
  }

  const engagementId = Number(route.paramMap.get('id'));

  return handoverService.getHandover(engagementId).pipe(
    map(() => true),
    catchError(() => {
      router.navigate(['/dashboard']);
      return of(false);
    })
  );
};
