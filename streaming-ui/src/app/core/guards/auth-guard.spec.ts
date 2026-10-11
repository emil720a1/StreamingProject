import { TestBed } from '@angular/core/testing';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  provideRouter,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import {
  firstValueFrom,
  Observable,
  of,
} from 'rxjs';

import { AuthService } from '../services/auth';
import { authGuard } from './auth-guard';

describe('authGuard', () => {
  let isAuthenticated = true;

  const authServiceMock = {
    isAuthenticated: () => of(isAuthenticated),
  };

  const executeGuard = () =>
    TestBed.runInInjectionContext(() =>
      authGuard(
        {} as ActivatedRouteSnapshot,
        {} as RouterStateSnapshot,
      ),
    );

  beforeEach(() => {
    isAuthenticated = true;

    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        {
          provide: AuthService,
          useValue: authServiceMock,
        },
      ],
    });
  });

  it('should allow authenticated user', async () => {
    isAuthenticated = true;

    const result = await firstValueFrom(
      executeGuard() as Observable<boolean | UrlTree>,
    );

    expect(result).toBe(true);
  });

  it('should redirect unauthenticated user to login', async () => {
    isAuthenticated = false;

    const result = await firstValueFrom(
      executeGuard() as Observable<boolean | UrlTree>,
    );
    const router = TestBed.inject(Router);

    expect(result).toBeInstanceOf(UrlTree);
    expect(router.serializeUrl(result as UrlTree)).toBe('/login');
  });
});
