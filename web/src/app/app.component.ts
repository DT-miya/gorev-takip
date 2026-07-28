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
          {{ isAdminArea() ? '👑 TaskBoard Admin' : 'Görev Takip' }}
        </span>

        <span class="spacer"></span>

        <!-- 🔴 KULLANICI / STANDART MENÜ (Sadece /admin rotası dışındayken) -->
        @if (!isAdminArea()) {
          <a mat-button routerLink="/dashboard" routerLinkActive="active">Panelim</a>
          <a mat-button routerLink="/projects" routerLinkActive="active">Projelerim</a>

          <!-- Eğer Kullanıcı Admin ise Admin Paneline Geçiş Butonu -->
          @if (authService.getUserRole() === 'Admin') {
            <a mat-button routerLink="/admin" class="nav-admin-link">
              👑 Admin Paneline Geç
            </a>
          }
        }

        <!-- 👑 ADMİN MENÜSÜ (Sadece /admin rotası ve altındayken) -->
        @if (isAdminArea()) {
          <a mat-button routerLink="/admin" routerLinkActive="active">
            📊 Admin Dashboard
          </a>
          <a mat-button routerLink="/admin-projects" routerLinkActive="active">
            📁 Projeler
          </a>
          <a mat-button routerLink="/admin-users" routerLinkActive="active">
            👥 Kullanıcılar
          </a>
          <a mat-button routerLink="/admin-stats" routerLinkActive="active">
            📈 İstatistikler
          </a>

          <span class="divider">|</span>
          <a mat-button routerLink="/dashboard" class="back-link">
            ↩️ Uygulamaya Dön
          </a>
        }

        <button mat-button (click)="logout()">Çıkış</button>
      </mat-toolbar>
    }

    <div class="page-content">
      <router-outlet></router-outlet>
    </div>


    <router-outlet></router-outlet>

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