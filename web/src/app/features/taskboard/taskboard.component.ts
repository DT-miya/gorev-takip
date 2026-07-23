import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '@services/auth.service';

@Component({
  selector: 'app-taskboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div style="max-width: 600px; margin: 40px auto; padding: 24px; background: white; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);">
      <h2>📋 Görev Panosu (TaskBoard)</h2>
      <p style="color: #16a34a; font-weight: 500; margin: 12px 0;">
        ✓ Tebrikler! JWT Token doğrulandı ve korumalı alana eriştiniz.
      </p>
      
      <button (click)="onLogout()" style="padding: 8px 16px; background: #dc2626; color: white; border: none; border-radius: 4px; cursor: pointer; margin-top: 16px;">
        Çıkış Yap (Logout)
      </button>
    </div>
  `
})
export class TaskboardComponent {
  constructor(private authService: AuthService, private router: Router) {}

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}