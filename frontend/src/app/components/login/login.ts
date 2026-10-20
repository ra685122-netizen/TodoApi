import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms'; //يسمح باستخدام ngModel لربط الحقول بالمتغيرات
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  username = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  constructor(private authService: AuthService) {}

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.username.trim() || !this.password) { //يزيل المسافات الزائدة من بداية اسم المستخدم ونهايته
      this.errorMessage = 'Please enter your username and password.';
      return;
    }

    this.isLoading = true;

    this.authService.login({
      username: this.username.trim(),
      password: this.password
    }).subscribe({                     //يتعامل مع نتيجة طلب HTTP
      next: () => {
        window.location.href = '/dashboard';
      },
      error: () => {
        this.errorMessage = 'Login failed. Check your username and password.';
        this.isLoading = false;
      }
    });
  }
}

