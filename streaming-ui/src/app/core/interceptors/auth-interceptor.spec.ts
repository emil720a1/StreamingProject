import {
  HttpErrorResponse,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import {
  of,
  Subject,
  throwError,
} from 'rxjs';

import { AuthService } from '../services/auth';
import { authInterceptor } from './auth-interceptor';

describe('authInterceptor', () => {
  const authServiceMock = {
    refreshToken: vi.fn(),
  };

  beforeEach(() => {
    vi.clearAllMocks();

    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthService,
          useValue: authServiceMock,
        },
      ],
    });
  });

  it('should refresh cookies and retry an API request after 401', () => {
    const request = new HttpRequest(
      'GET',
      'http://localhost:5228/api/Streams',
    );
    const unauthorizedError = new HttpErrorResponse({ status: 401 });
    const next = vi.fn()
      .mockReturnValueOnce(throwError(() => unauthorizedError))
      .mockReturnValueOnce(of(new HttpResponse({ status: 200 })));

    authServiceMock.refreshToken.mockReturnValue(of(undefined));

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe();
    });

    expect(authServiceMock.refreshToken).toHaveBeenCalledOnce();
    expect(next).toHaveBeenCalledTimes(2);

    const retriedRequest = next.mock.calls[1][0];
    expect(retriedRequest.withCredentials).toBe(true);
    expect(retriedRequest.headers.has('Authorization')).toBe(false);
  });

  it('should propagate an error when cookie refresh fails', () => {
    const request = new HttpRequest(
      'GET',
      'http://localhost:5228/api/Streams',
    );
    const unauthorizedError = new HttpErrorResponse({ status: 401 });
    const next = vi.fn(() => throwError(() => unauthorizedError));

    authServiceMock.refreshToken.mockReturnValue(
      throwError(() => unauthorizedError),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe({ error: () => {} });
    });

    expect(authServiceMock.refreshToken).toHaveBeenCalledOnce();
    expect(next).toHaveBeenCalledTimes(1);
  });

  it('should share one refresh request between concurrent 401 responses', () => {
    const firstRequest = new HttpRequest(
      'GET',
      'http://localhost:5228/api/Streams/1',
    );
    const secondRequest = new HttpRequest(
      'GET',
      'http://localhost:5228/api/Streams/2',
    );
    const refreshResponse$ = new Subject<void>();
    const unauthorizedError = new HttpErrorResponse({ status: 401 });
    const next = vi.fn()
      .mockReturnValueOnce(throwError(() => unauthorizedError))
      .mockReturnValueOnce(throwError(() => unauthorizedError))
      .mockReturnValue(of(new HttpResponse({ status: 200 })));

    authServiceMock.refreshToken.mockReturnValue(
      refreshResponse$.asObservable(),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(firstRequest, next).subscribe();
      authInterceptor(secondRequest, next).subscribe();
    });

    expect(authServiceMock.refreshToken).toHaveBeenCalledOnce();

    refreshResponse$.next();
    refreshResponse$.complete();

    expect(next).toHaveBeenCalledTimes(4);
  });

  it('should not refresh after a failed login request', () => {
    const request = new HttpRequest(
      'POST',
      'http://localhost:5228/api/Auth/login',
      {},
    );
    const next = vi.fn(() =>
      throwError(() => new HttpErrorResponse({ status: 401 })),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe({ error: () => {} });
    });

    expect(authServiceMock.refreshToken).not.toHaveBeenCalled();
  });

  it('should not add an Authorization header to API requests', () => {
    const request = new HttpRequest(
      'GET',
      'http://localhost:5228/api/Streams',
    );
    const next = vi.fn((forwardedRequest: HttpRequest<unknown>) =>
      of(new HttpResponse({ status: 200 })),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe();
    });

    const forwardedRequest = next.mock.calls[0][0];
    expect(forwardedRequest.headers.has('Authorization')).toBe(false);
  });

  it('should enable credentials for backend API requests', () => {
    const request = new HttpRequest(
      'POST',
      'http://localhost:5228/api/Auth/login',
      {},
    );
    const next = vi.fn((forwardedRequest: HttpRequest<unknown>) =>
      of(new HttpResponse({ status: 200 })),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe();
    });

    expect(next.mock.calls[0][0].withCredentials).toBe(true);
  });

  it('should not enable credentials or refresh for external requests', () => {
    const request = new HttpRequest(
      'GET',
      'https://example.com/resource',
    );
    const next = vi.fn((forwardedRequest: HttpRequest<unknown>) =>
      of(new HttpResponse({ status: 200 })),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe();
    });

    expect(next.mock.calls[0][0].withCredentials).toBe(false);
    expect(authServiceMock.refreshToken).not.toHaveBeenCalled();
  });
});
