import { ChangeDetectorRef, Component } from '@angular/core';
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
  successMessage: string = '';
  isLoading: boolean = false;

  constructor(private authService: AuthService, private router: Router,  private cdr: ChangeDetectorRef) {}

  onRegister(): void {
    if (!this.model.fullName || !this.model.email || !this.model.password) {
      this.errorMessage = 'Lütfen tüm alaları doldurun.';
      this.cdr.detectChanges();
      return;
    }


    // 2. Geçerli bir e-posta mı kontrolü (Bunu mutlaka eklemelisiniz)
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(this.model.email)) {
    this.errorMessage = 'Lütfen geçerli bir e-posta adresi girin.';
    this.cdr.detectChanges();
    return;
  }

    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.authService.register(this.model).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.errorMessage = '';
        this.successMessage = 'Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz...'
        this.cdr.detectChanges();
        setTimeout(() => {
        this.router.navigate(['/login']);
        }, 3000);
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Kayıt olunamadı.';
        this.cdr.detectChanges();
      }
    });
  }
}