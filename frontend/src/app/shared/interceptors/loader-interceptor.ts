import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoaderService } from '@shared/services/loader.service';
import { finalize } from 'rxjs';

let activeRequests = 0;

export const loaderInterceptor: HttpInterceptorFn = (req, next) => {
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
