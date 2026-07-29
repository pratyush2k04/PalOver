import { HttpInterceptorFn, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, tap, throwError } from 'rxjs';

let inMemoryToken: string | null = null;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  // Attach stored Authorization header if present, plus withCredentials
  let authReq = req.clone({
    withCredentials: true
  });

  if (inMemoryToken) {
    authReq = authReq.clone({
      setHeaders: {
        Authorization: inMemoryToken
      }
    });
  }

  return next(authReq).pipe(
    tap((event) => {
      if (event instanceof HttpResponse) {
        const authHeader = event.headers.get('Authorization');
        if (authHeader) {
          inMemoryToken = authHeader;
        }
      }
    }),
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        inMemoryToken = null;
        router.navigate(['/login'], {
          queryParams: { reason: 'session_expired' }
        });
      }
      return throwError(() => error);
    })
  );
};
