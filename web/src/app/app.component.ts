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
      <mat-toolbar color="primary">
        <span class="app-title">Görev Takip</span>
        <span class="spacer"></span>
        <a mat-button routerLink="/dashboard">Panelim</a>
        <a mat-button routerLink="/projects">Projelerim</a>
        <button mat-button (click)="logout()">Çıkış</button>
      </mat-toolbar>
    }
    <router-outlet></router-outlet>
  `,
  styles: [`
    .spacer { flex: 1 1 auto; }
    .app-title { font-weight: 600; }
  `]
})
export class AppComponent {
  authService = inject(AuthService);
  private router = inject(Router);

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}