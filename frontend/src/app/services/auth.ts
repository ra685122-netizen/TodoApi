import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  Observable,                                                                                                                                                                               //يمثل عملية غير متزامنة قد ترجع نتيجة لاحقًا، مثل طلب تسجيل الدخول.
  tap,                                                                                                                                                                                      //ينفذ إجراءً جانبيًا عند وصول النتيجة، مثل حفظ التوكن.
  catchError,                                                                                                                                                                               //يعالج الأخطاء التي تحدث أثناء الطلب.
  throwError,                                                                                                                                                                               //يعيد الخطأ إلى الجزء الذي استدعى العملية.
  finalize,                                                                                                                                                                                 //ينفذ إجراءً عند انتهاء العملية، سواء نجحت أو فشلت أو أُلغيت.
  shareReplay                                                                                                                                                                               //يسمح لعدة مشتركين بمشاركة نتيجة العملية نفسها بدل تنفيذ طلبات مكررة.
} from 'rxjs';

import { LoginRequest } from '../models/login-request';
import { TokenResponse } from '../models/token-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5047/Authentication';

  private refreshRequest$: Observable<TokenResponse> | null = null;

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<TokenResponse> {
    return this.http
      .post<TokenResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap(response => this.saveTokens(response))
      );
  }

  refreshToken(): Observable<TokenResponse> {
    if (this.refreshRequest$) {
      return this.refreshRequest$;
    }

    const refreshToken = localStorage.getItem('refreshToken');

    if (!refreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token found.'));
    }

    this.refreshRequest$ = this.http
      .post<TokenResponse>(
        `${this.apiUrl}/token/refresh`,
        { refreshToken }
      )
      .pipe(
        tap(response => this.saveTokens(response)),
        catchError(error => {
          this.logout();
          return throwError(() => error);
        }),
        finalize(() => {
          this.refreshRequest$ = null;
        }),
        shareReplay({ bufferSize: 1, refCount: false })
      );

    return this.refreshRequest$;
  }

  private saveTokens(response: TokenResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isTokenExpired(): boolean {
    const token = this.getToken();

    if (!token) {
      return true;
    }
    //تقرأ JWT
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000;

      return Date.now() >= expirationTime;
    } catch {
      return true;
    }
  }

  isLoggedIn(): boolean {
    return !!this.getToken() && !this.isTokenExpired();
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
  }
}

