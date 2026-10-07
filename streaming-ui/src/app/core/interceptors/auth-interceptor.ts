import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import {
  catchError,
  finalize,
  Observable,
  shareReplay,
  switchMap,
  throwError,
} from 'rxjs';

import { AuthService } from '../services/auth';
import { AuthResponse } from '../../shared/models/auth';

let refreshRequest$: Observable<AuthResponse> | null = null;

const getRefreshRequest = (
  authService: AuthService,
): Observable<AuthResponse> => {
  if (!refreshRequest$) {
    const refreshToken = authService.getRefreshToken();

    if (!refreshToken) {
      return throwError(() => new Error('Refresh token is missing'));
    }

    refreshRequest$ = authService.refreshToken(refreshToken).pipe(
      finalize(() => {
        refreshRequest$ = null;
      }),
      shareReplay({
        bufferSize: 1,
        refCount: false,
      }),
    );
  }

  return refreshRequest$;
};

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

      if (!authService.getRefreshToken()) {
        authService.logout();
        return throwError(() => error);
      }

      return getRefreshRequest(authService).pipe(
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
