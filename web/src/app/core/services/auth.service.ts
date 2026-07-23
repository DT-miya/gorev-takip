import { LoginRequest, RegisterRequest } from '@app/core/models/auth.model';
import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '@environment/environment';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Sizin .NET HTTP Portunuz
  private apiUrl = `${environment.apiUrl}`;
  
  isLoggedIn = signal<boolean>(!!localStorage.getItem('token'));

  constructor(private http: HttpClient) {}

  login(model: LoginRequest): Observable<any> {
  return this.http.post<any>(`${this.apiUrl}/Auth/login`, model).pipe(
    tap(response => {
      if (response && response.token) {
        // Token'ı localStorage'a kaydediyoruz
        localStorage.setItem('token', response.token);
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
}