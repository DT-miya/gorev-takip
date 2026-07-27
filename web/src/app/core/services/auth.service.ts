import { LoginRequest, RegisterRequest } from '@models/auth.service.model';
import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '@env/environment';

export type { LoginRequest, RegisterRequest };

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}`;
  
  isLoggedIn = signal<boolean>(!!localStorage.getItem('token'));

  constructor(private http: HttpClient) {}

  login(model: LoginRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Auth/login`, model).pipe(
      tap(response => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          this.isLoggedIn.set(true);
        }
      })
    );
  }

  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/Auth/register`, data);
  }

  logout(): void {
    localStorage.removeItem('token');
    this.isLoggedIn.set(false);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

// 🚀 TOKEN İÇİNDEN ROL (ROLE) OKUMA METODU
getUserRole(): string | null {
  const token = this.getToken();
  if (!token) return null;

  try {
    const payloadBase64 = token.split('.')[1];
    // UTF-8 karakter desteğiyle token payload'ını çözüyoruz
    const payloadJson = decodeURIComponent(
      atob(payloadBase64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    const payload = JSON.parse(payloadJson);

    // Payload anahtarlarından sonu "role" ile biteni bul (Link yazmaya gerek kalmaz)
    const roleKey = Object.keys(payload).find(key => key.toLowerCase().endsWith('role'));
    
    return roleKey ? payload[roleKey] : null;
  } catch (e) {
    console.error('Token okunamadı:', e);
    return null;
  }
}

}