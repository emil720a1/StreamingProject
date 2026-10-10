import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  catchError,
  map,
  Observable,
  of,
} from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  LoginRequest,
  RegisterRequest,
} from '../../shared/models/auth';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/Auth`;

  login(request: LoginRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/login`, request);
  }

  register(request: RegisterRequest): Observable<unknown> {
    return this.http.post(
      `${this.apiUrl}/register`,
      request,
    );
  }

  refreshToken(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/refresh-token`, {});
  }

  isAuthenticated(): Observable<boolean> {
    return this.http.get<void>(`${this.apiUrl}/session`).pipe(
      map(() => true),
      catchError(() => of(false)),
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/logout`, {});
  }
}
