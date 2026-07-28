import { ChangeDetectorRef, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, LoginRequest } from '@services/auth.service';

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

  constructor(
    private authService: AuthService, 
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  onLogin(): void {
    if (!this.model.email || !this.model.password) {
      this.errorMessage = 'Lütfen tüm alanları doldurun.';
      this.cdr.detectChanges(); // Hata mesajını hemen göstermek için değişiklik algılamayı tetikleyin
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges(); // Yükleniyor durumunu hemen göstermek için değişiklik algılamayı tetikleyin

    this.authService.login(this.model).subscribe({
     next: () => {
  this.isLoading = false;
  this.router.navigate(['/dashboard']); // Başarılı giriş sonrası yönlendirme
},
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Giriş yapılamadı. Bilgilerinizi kontrol edin.';

        this.cdr.detectChanges(); // Hata mesajını hemen göstermek için değişiklik algılamayı tetikleyin
      }
    });
  }
}