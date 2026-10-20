
import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Dashboard } from './components/dashboard/dashboard';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  {
    path: 'login',
    component: Login
  },
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard]                                                                                                                                   //يشترط اجتياز Guard قبل فتح Dashboard
  },
  {
    path: '',
    redirectTo: 'login',                                                                                                                                        //حوّل المسار الرئيسي إلى تسجيل الدخول.
    pathMatch: 'full'                                                                                                                                           //يشترط تطابق المسار كاملًا مع المسار الفارغ
  },
  {
    path: '**',                                                                                                                                              // يلتقط المسارات غير المعروفة ويوجهها إلى Login
    redirectTo: 'login'
  }
];

