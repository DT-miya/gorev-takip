import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, RegisterRequest } from '@services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  model: RegisterRequest = { fullName: '', email: '', password: '' };
  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  onRegister(): void {
    if (!this.model.fullName || !this.model.email || !this.model.password) {
      this.errorMessage = 'Lütfen tüm alaları doldurun.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.register(this.model).subscribe({
      next: () => {
        this.isLoading = false;
        alert('Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz.');
        this.router.navigate(['/login']);
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Kayıt olunamadı.';
      }
    });
  }
}