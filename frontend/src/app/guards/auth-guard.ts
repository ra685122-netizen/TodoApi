import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  }

  if (localStorage.getItem('refreshToken')) {
    return authService.refreshToken().pipe(
      map(() => true),
      catchError(() => of(router.createUrlTree(['/login'])))                                                                                              //يتعامل مع فشل تجديد التوكن.//يحوّل وجهة تسجيل الدخول إلى نتيجة Observable تناسب الحارس
    );
  }

  return router.createUrlTree(['/login']);
};



