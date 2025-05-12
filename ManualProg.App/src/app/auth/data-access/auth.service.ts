import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, of, tap, throwError } from 'rxjs';
import { ILoginRequest } from './requests/login-request';
import { ITokenResponse } from './responses/token-response';
import { IUser } from '../../shared/models/user';
import { IRefreshTokenRequest } from './requests/refresh-token-request';
import { ILogoutRequest } from './requests/logout-request';
import { IRegisterRequest } from './requests/register-request';
import { Utils } from '../../utils';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  currentUserSignal = signal<IUser | null>(null);

  private readonly http = inject(HttpClient);
  private readonly apiUrl = "/api/auth";

  private readonly tokenKey = 'token';
  private readonly refreshTokenKey = 'refresh-token';
  private readonly userKey = 'user';

  login(data: ILoginRequest): Observable<ITokenResponse> {
    const url = `${this.apiUrl}/login`;

    return this.http.post<ITokenResponse>(url, data)
      .pipe(
        tap(data => this.setTokens(data))
      );
  }

  register(data: IRegisterRequest): Observable<ITokenResponse> {
    const url = `${this.apiUrl}/register`;

    const formData = Utils.objectToFormData(data);

    return this.http.post<ITokenResponse>(url, formData)
      .pipe(
        tap(data => this.setTokens(data))
      );
  }

  logout(): Observable<any> {
    const refreshToken = localStorage.getItem(this.refreshTokenKey);

    if (!refreshToken) {
      this.setTokens(null);

      return of();
    }

    const url = `${this.apiUrl}/logout`;

    const data = <ILogoutRequest>{
      refreshToken: refreshToken
    };

    return this.http.post<any>(url, data)
      .pipe(
        tap(() => this.setTokens(null)),
        catchError((error) => {
          this.setTokens(null);

          return throwError(() => error);
        })
      );
  }

  refresh(): Observable<ITokenResponse> {
    const refreshToken = localStorage.getItem(this.refreshTokenKey);

    if (!refreshToken) {
      this.setTokens(null);

      return of();
    }

    const url = `${this.apiUrl}/refresh`;

    const data = <IRefreshTokenRequest>{
      refreshToken: refreshToken
    };

    return this.http.post<ITokenResponse>(url, data)
      .pipe(
        tap(data => this.setTokens(data)),
        catchError((error) => {
          this.setTokens(null);

          return throwError(() => error);
        })
      );
  }

  getToken(): string | null {
    const token = localStorage.getItem(this.tokenKey);

    return token ? token : null;
  }

  initUser(): void {
    const storedUser = localStorage.getItem(this.userKey);

    if (!storedUser) {
      this.setTokens(null);

      return;
    }

    this.refresh().subscribe(() => {});
  }

  private setTokens(data: ITokenResponse | null): void {
    if (data === null) {
      localStorage.removeItem(this.tokenKey);
      localStorage.removeItem(this.refreshTokenKey);

      this.setUser(null);
    } else {
      localStorage.setItem(this.tokenKey, data.accessToken);
      localStorage.setItem(this.refreshTokenKey, data.refreshToken);

      this.setUser(<IUser>{
        id: data.userId,
        username: data.username,
        role: data.userRole,
        profileId: data.profileId
      });
    }
  }

  private setUser(user: IUser | null): void {
    this.currentUserSignal.set(user);

    if (user === null) {
      localStorage.removeItem(this.userKey);
    } else {
      localStorage.setItem(this.userKey, JSON.stringify(user));
    }
  }
}
