import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  // Ensure cookies (HttpOnly auth cookie) are included in all HTTP requests
  const authReq = req.clone({
    withCredentials: true
  });

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Redirect to login page on session expiration / unauthorized access
        router.navigate(['/login'], {
          queryParams: { reason: 'session_expired' }
        });
      }
      return throwError(() => error);
    })
  );
};
