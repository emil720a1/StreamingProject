import { TestBed } from '@angular/core/testing';
import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { AuthService } from './auth';

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should send login credentials to the API', () => {
    const request = {
      email: 'alex@example.com',
      password: 'password123',
    };

    service.login(request).subscribe();

    const testRequest = http.expectOne('http://localhost:5228/api/Auth/login');
    expect(testRequest.request.method).toBe('POST');
    expect(testRequest.request.body).toEqual(request);
    testRequest.flush({ token: 'token', refreshToken: 'refresh-token' });
  });

  it('should refresh tokens and store new ones', () => {
    const oldRefreshToken = 'old-refresh-token';
    const response = {
      token: 'new-access-token',
      refreshToken: 'new-refresh-token',
    };

    service.refreshToken(oldRefreshToken).subscribe((result) => {
      expect(result).toEqual(response);
    });

    const testRequest = http.expectOne(
      'http://localhost:5228/api/Auth/refresh-token',
    );

    expect(testRequest.request.method).toBe('POST');
    expect(testRequest.request.body).toEqual({
      refreshToken: oldRefreshToken,
    });

    testRequest.flush(response);

    expect(localStorage.getItem('access_token'))
      .toBe('new-access-token');

    expect(localStorage.getItem('refresh_token'))
      .toBe('new-refresh-token');
  });

  it('should send registration data to the API', () => {
    const request = {
      username: 'alex',
      email: 'alex@example.com',
      password: 'password123',
    };

    service.register(request).subscribe();

    const testRequest = http.expectOne('http://localhost:5228/api/Auth/register');
    expect(testRequest.request.method).toBe('POST');
    expect(testRequest.request.body).toEqual(request);
    testRequest.flush({});
  });

  it('should call logout endpoint and clear stored tokens', () => {
    localStorage.setItem('access_token', 'access-token');
    localStorage.setItem('refresh_token', 'refresh-token');

    service.logout().subscribe();

    const testRequest = http.expectOne(
      'http://localhost:5228/api/Auth/logout',
    );

    expect(testRequest.request.method).toBe('POST');
    testRequest.flush(null);

    expect(localStorage.getItem('access_token')).toBeNull();
    expect(localStorage.getItem('refresh_token')).toBeNull();
  });
});
