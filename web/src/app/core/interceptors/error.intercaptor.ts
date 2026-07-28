import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // 🚀 1. /auth/login ve /auth/register isteklerinde SnackBar GÖSTERME
      const isAuthRequest = req.url.includes('/login') || req.url.includes('/register');

      if (!isAuthRequest) {
        const message = error.error?.message || 'Bir hata oluştu.';
        snackBar.open(message, 'Kapat', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
      }

      // 🚀 2. Hatayı bileşene (LoginComponent vb.) iletmeye devam et
      return throwError(() => error);
    })
  );
};