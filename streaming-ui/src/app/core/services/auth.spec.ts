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
});
