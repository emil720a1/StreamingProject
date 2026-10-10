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
  let token: string | null;
  let refreshToken: string | null;

  const authServiceMock = {
    getToken: () => token,
    getRefreshToken: () => refreshToken,
    refreshToken: vi.fn(),
    clearSession: vi.fn(),
  };

  beforeEach(() => {
    token = 'test-token';
    refreshToken = 'test-refresh-token';
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

  it('should refresh token and retry request after 401', () => {
    const request = new HttpRequest(
      'GET',
      '/api/streams',
    );

    const refreshResponse = {
      token: 'new-token',
      refreshToken: 'new-refresh-token',
    };

    const next = vi.fn()
      .mockReturnValueOnce(
        throwError(() => new HttpErrorResponse({
          status: 401,
          statusText: 'Unauthorized',
        })),
      )
      .mockReturnValueOnce(
        of(
          new HttpResponse({
            status: 200,
            body: {},
          }),
        ),
      );

    authServiceMock.refreshToken
      .mockReturnValue(of(refreshResponse));

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe();
    });

    expect(authServiceMock.refreshToken)
      .toHaveBeenCalledWith('test-refresh-token');

    expect(next).toHaveBeenCalledTimes(2);

    const retriedRequest = next.mock.calls[1][0];

    expect(
      retriedRequest.headers.get('Authorization'),
    ).toBe('Bearer new-token');
  });

  it('should clear the session when refresh token request fails', () => {
    const request = new HttpRequest(
      'GET',
      '/api/streams',
    );

    const unauthorizedError = new HttpErrorResponse({
      status: 401,
      statusText: 'Unauthorized',
    });

    const next = vi.fn(() =>
      throwError(() => unauthorizedError),
    );

    authServiceMock.refreshToken
      .mockReturnValue(
        throwError(() => unauthorizedError),
      );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe({
        error: () => {},
      });
    });

    expect(authServiceMock.refreshToken)
      .toHaveBeenCalledWith('test-refresh-token');

    expect(authServiceMock.clearSession)
      .toHaveBeenCalled();

    expect(next).toHaveBeenCalledTimes(1);
  });

  it('should share one refresh request between concurrent 401 responses', () => {
    const firstRequest = new HttpRequest(
      'GET',
      '/api/streams/1',
    );
    const secondRequest = new HttpRequest(
      'GET',
      '/api/streams/2',
    );
    const refreshResponse$ = new Subject<{
      token: string;
      refreshToken: string;
    }>();
    const unauthorizedError = new HttpErrorResponse({
      status: 401,
      statusText: 'Unauthorized',
    });

    const next = vi.fn()
      .mockReturnValueOnce(throwError(() => unauthorizedError))
      .mockReturnValueOnce(throwError(() => unauthorizedError))
      .mockReturnValue(
        of(
          new HttpResponse({
            status: 200,
            body: {},
          }),
        ),
      );

    authServiceMock.refreshToken
      .mockReturnValue(refreshResponse$.asObservable());

    TestBed.runInInjectionContext(() => {
      authInterceptor(firstRequest, next).subscribe();
      authInterceptor(secondRequest, next).subscribe();
    });

    expect(authServiceMock.refreshToken)
      .toHaveBeenCalledTimes(1);

    refreshResponse$.next({
      token: 'new-token',
      refreshToken: 'new-refresh-token',
    });
    refreshResponse$.complete();

    expect(next).toHaveBeenCalledTimes(4);
    expect(next.mock.calls[2][0].headers.get('Authorization'))
      .toBe('Bearer new-token');
    expect(next.mock.calls[3][0].headers.get('Authorization'))
      .toBe('Bearer new-token');
  });

  it('should clear the session when there is no refresh token', () => {
    refreshToken = null;

    const request = new HttpRequest(
      'GET',
      '/api/streams',
    );
    const unauthorizedError = new HttpErrorResponse({
      status: 401,
      statusText: 'Unauthorized',
    });
    const next = vi.fn(() =>
      throwError(() => unauthorizedError),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next).subscribe({
        error: () => {},
      });
    });

    expect(authServiceMock.refreshToken)
      .not.toHaveBeenCalled();
    expect(authServiceMock.clearSession)
      .toHaveBeenCalledTimes(1);
  });

  it('should add Authorization header when token exists', () => {
    const request = new HttpRequest(
      'GET',
      '/api/streams',
    );

    const next = vi.fn((forwardedRequest: HttpRequest<unknown>) =>
      of(
        new HttpResponse({
          status: 200,
          body: {},
        }),
      ),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next);
    });

    expect(next).toHaveBeenCalledTimes(1);

    const forwardedRequest = next.mock.calls[0][0];

    expect(
      forwardedRequest.headers.get('Authorization'),
    ).toBe('Bearer test-token');
  });

  it('should not add Authorization header when token does not exist', () => {
    token = null;

    const request = new HttpRequest(
      'GET',
      '/api/streams',
    );

    const next = vi.fn((forwardedRequest: HttpRequest<unknown>) =>
      of(
        new HttpResponse({
          status: 200,
          body: {},
        }),
      ),
    );

    TestBed.runInInjectionContext(() => {
      authInterceptor(request, next);
    });

    expect(next).toHaveBeenCalledTimes(1);

    const forwardedRequest = next.mock.calls[0][0];

    expect(
      forwardedRequest.headers.has('Authorization'),
    ).toBe(false);
  });

  it('should enable credentials for backend API requests', () => {
    token = null;

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

  it('should not enable credentials for external requests', () => {
    token = null;

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
  });
});
