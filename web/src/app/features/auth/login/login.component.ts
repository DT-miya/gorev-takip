import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, LoginRequest } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  model: LoginRequest = { email: '', password: '' };
  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  onLogin(): void {
    if (!this.model.email || !this.model.password) {
      this.errorMessage = 'Lütfen tüm alanları doldurun.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
console.log("Gönderilen Login Verisi:", this.model);
    this.authService.login(this.model).subscribe({
     next: () => {
  this.isLoading = false;
  this.router.navigate(['/taskboard']);
},
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Giriş yapılamadı. Bilgilerinizi kontrol edin.';
      }
    });
  }
}