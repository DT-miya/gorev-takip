import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '@services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Kullanıcı giriş yapmış mı ve rolü 'Admin' mi?
  if (authService.isLoggedIn() && authService.getUserRole() === 'Admin') {
    return true;
  }

  // Admin değilse ana sayfaya yönlendir
  router.navigate(['/projects']);
  return false;
};