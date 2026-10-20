import {
  HttpInterceptorFn,                                                                                                                                          //هو نوع الدالة التي يستخدمها Angular للاعتراض على طلبات HTTP
  HttpErrorResponse
} from '@angular/common/http';

import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth';


export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const isLoginRequest = request.url.endsWith('/Authentication/login');
  const isRefreshRequest = request.url.endsWith('/Authentication/token/refresh');

  const token = authService.getToken();                                                                                                                      //يقرأ Access Token المحفوظ

  const authorizedRequest =
    token && !isLoginRequest && !isRefreshRequest
      ? request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      })
      : request;

  return next(authorizedRequest).pipe(                                                                                                                         //يسمح بتركيب عمليات RxJS لمعالجة نتيجة
    catchError((error: HttpErrorResponse) => {
      if (
        error.status !== 401 ||
        isLoginRequest ||
        isRefreshRequest
      ) {
        return throwError(() => error);
      }
      // إذا لم يوجد Refresh Token، نمسح بيانات الدخول ونعيد المستخدم إلى Login
      if (!localStorage.getItem('refreshToken')) {
        authService.logout();
        router.navigate(['/login']);
        return throwError(() => error);
      }

      return authService.refreshToken().pipe(
        switchMap(response => {
          const retryRequest = request.clone({
            setHeaders: {
              Authorization: `Bearer ${response.token}`
            }
          });

          return next(retryRequest);
        }),
        catchError(refreshError => {
          authService.logout();
          router.navigate(['/login']); // إعادة المستخدم إلى صفحة تسجيل الدخول
          return throwError(() => refreshError);
        })
      );
    })
  );
};

