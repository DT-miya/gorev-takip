import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, Router } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '@services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, MatToolbarModule, MatButtonModule],
  template: `
    @if (authService.isLoggedIn()) {
      <mat-toolbar [class.admin-toolbar]="isAdminArea()">
        <span class="app-title">
          {{ isAdminArea() ? 'TaskBoard Admin' : 'Görev Takip' }}
        </span>

        <span class="spacer"></span>

        <!-- 🔴 KULLANICI / STANDART MENÜ (Sadece /admin rotası dışındayken) -->
        @if (!isAdminArea()) {
          <a mat-button routerLink="/dashboard" routerLinkActive="active"> <i class="fas fa-tachometer-alt"></i> Panelim</a>
          <a mat-button routerLink="/projects" routerLinkActive="active"> <i class="fas fa-folder"></i> Projelerim</a>
          <a mat-button routerLink="/profile" routerLinkActive="active"> <i class="fas fa-user"></i> Profilim</a>

          <!-- Eğer Kullanıcı Admin ise Admin Paneline Geçiş Butonu -->
          @if (authService.getUserRole() === 'Admin') {
            <a mat-button routerLink="/admin" class="nav-admin-link">
              <i class="fas fa-crown"></i> Admin Paneline Geç
            </a>
          }
        }

        <!-- 🟡 ADMİN MENÜSÜ (Sadece /admin rotası ve altındayken) -->
        @if (isAdminArea()) {
          <a mat-button routerLink="/admin" routerLinkActive="active">
            <i class="fas fa-home"></i> Admin Anasayfa
          </a>
          <a mat-button routerLink="/admin-projects" routerLinkActive="active">
            <i class="fas fa-folder"></i> Projeler
          </a>
          <a mat-button routerLink="/admin-users" routerLinkActive="active">
            <i class="fas fa-users"></i> Kullanıcılar
          </a>
          <a mat-button routerLink="/admin-stats" routerLinkActive="active">
            <i class="fas fa-chart-line"></i> İstatistikler
          </a>

          <a mat-button routerLink="/admin-logs" routerLinkActive="active">
            <i class="fas fa-info-circle"></i> İşlem Logları
          </a>

          <span class="divider">|</span>
          <a mat-button routerLink="/dashboard" class="back-link">
            <i class="fas fa-arrow-left"></i> Uygulamaya Dön
          </a>
        }

        <button mat-button (click)="logout()">
          <i class="fas fa-sign-out-alt"></i> Çıkış
        </button>
      </mat-toolbar>

      <!-- Giriş Yapmış Kullanıcılar İçin Sayfa Kapsayıcısı -->
      <div class="page-content">
        <router-outlet></router-outlet>
      </div>
    } @else {
      <!-- Giriş Yapmamış (Login/Register) Kullanıcılar İçin Doğrudan Gösterim -->
      <router-outlet></router-outlet>
    }
  `,
  styleUrl: './app.component.css'
})
export class AppComponent {
  authService = inject(AuthService);
  private router = inject(Router);

  // 🚀 Admin alanında mıyız kontrolü
  isAdminArea(): boolean {
    return this.router.url.startsWith('/admin') && this.authService.getUserRole() === 'Admin';
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}