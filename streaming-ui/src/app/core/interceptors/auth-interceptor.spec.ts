import {
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { AuthService } from '../services/auth';
import { authInterceptor } from './auth-interceptor';

describe('authInterceptor', () => {
  let token: string | null;

  const authServiceMock = {
    getToken: () => token,
  };

  beforeEach(() => {
    token = 'test-token';

    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthService,
          useValue: authServiceMock,
        },
      ],
    });
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
});
