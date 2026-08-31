import { HttpContextToken, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoaderService } from '@shared/services/loader.service';
import { finalize } from 'rxjs';

let activeRequests = 0;

export const SKIP_LOADER = new HttpContextToken<boolean>(() => false);

export const loaderInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.context.get(SKIP_LOADER)) {
    return next(req);
  }

  const loaderService = inject(LoaderService);

  if (activeRequests === 0) {
    loaderService.show();
  }
  activeRequests++;

  return next(req).pipe(
    finalize(() => {
      activeRequests--;
      if (activeRequests <= 0) {
        activeRequests = 0;
        loaderService.hide();
      }
    })
  );
};
