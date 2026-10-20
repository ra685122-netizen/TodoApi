
import {
  HttpInterceptorFn,                                                                                                                                          //هو نوع الدالة التي يستخدمها Angular للاعتراض على طلبات HTTP
  HttpContextToken,                                                                                                                                           //يساعدنا على تسجيل أن الطلب أُعيدت محاولته بالفعل
  HttpContext,
  HttpErrorResponse
} from '@angular/common/http';

import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth';

const AUTH_RETRIED = new HttpContextToken<boolean>(() => false);                                                                                              //منع تكرار إعادة الطلب

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);

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
      const alreadyRetried = request.context.get(AUTH_RETRIED);

      if (
        error.status !== 401 ||
        isLoginRequest ||
        isRefreshRequest ||
        alreadyRetried ||
        !token
      ) {
        return throwError(() => error);
      }

      return authService.refreshToken().pipe(
        switchMap(response => {                                                                                                                              // يستخدم الرمز الجديد لإعادة إرسال الطلب الأصلي
          const retryContext = new HttpContext()
            .set(AUTH_RETRIED, true);                                                                                                                         //منع تكرار محاولات التجديد بلا نهاية للطلب نفسه

          const retryRequest = request.clone({
            context: retryContext,
            setHeaders: {
              Authorization: `Bearer ${response.token}`
            }
          });

          return next(retryRequest);
        }),
        catchError(refreshError => {
          authService.logout();
          return throwError(() => refreshError);
        })
      );
    })
  );
};

