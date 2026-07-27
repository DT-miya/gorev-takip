import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { AdminUser, AdminStats } from '../models/admin.model';

@Injectable({
    providedIn: 'root'
})
export class AdminService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(`${this.apiUrl}/admin/users`);
  }

  getStats(): Observable<AdminStats> {
    return this.http.get<AdminStats>(`${this.apiUrl}/admin/stats`);
  }

  updateUserRole(userId: number, role: string): Observable<AdminUser[]> {
  return this.http.put<AdminUser[]>(`${this.apiUrl}/admin/users/${userId}/role`, { role });
  }
}