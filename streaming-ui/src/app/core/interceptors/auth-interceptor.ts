import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import {
  catchError,
  finalize,
  Observable,
  shareReplay,
  switchMap,
  throwError,
} from 'rxjs';

import { AuthService } from '../services/auth';

let refreshRequest$: Observable<void> | null = null;

const getRefreshRequest = (
  authService: AuthService,
): Observable<void> => {
  if (!refreshRequest$) {
    refreshRequest$ = authService.refreshToken().pipe(
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
  const isApiRequest = request.url.startsWith(environment.apiUrl);

  const requestWithCredentials = isApiRequest
    ? request.clone({ withCredentials: true })
    : request;

  return next(requestWithCredentials).pipe(
    catchError((error) => {
      const isAuthenticationRequest =
        request.url.includes('/Auth/login')
        || request.url.includes('/Auth/register')
        || request.url.includes('/Auth/refresh-token')
        || request.url.includes('/Auth/logout');

      if (
        !isApiRequest
        || error.status !== 401
        || isAuthenticationRequest
      ) {
        return throwError(() => error);
      }

      return getRefreshRequest(authService).pipe(
        switchMap(() => next(requestWithCredentials)),
        catchError((refreshError) =>
          throwError(() => refreshError),
        ),
      );
    }),
  );
};
