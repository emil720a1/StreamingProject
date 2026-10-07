import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import {
  catchError,
  switchMap,
  throwError,
} from 'rxjs';

import { AuthService } from '../services/auth';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (!token) {
    return next(request);
  }

  const requestWithToken = request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(requestWithToken).pipe(
    catchError((error) => {
      if (
        error.status !== 401
        || request.url.includes('/Auth/refresh-token')
      ) {
        return throwError(() => error);
      }

      const refreshToken = authService.getRefreshToken();

      if (!refreshToken) {
        authService.logout();
        return throwError(() => error);
      }

      return authService.refreshToken(refreshToken).pipe(
        switchMap((response) => {
          const retryRequest = request.clone({
            setHeaders: {
              Authorization: `Bearer ${response.token}`,
            },
          });

          return next(retryRequest);
        }),
        catchError((refreshError) => {
          authService.logout();
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
