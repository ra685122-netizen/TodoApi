
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);//نقلك إلى صفحة تسجيل الدخول دون إعادة تحميل التطبيق كاملًا
  }
}



