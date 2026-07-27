import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { AdminUser, AdminStats, AdminProject } from '../models/admin.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/admin`;

  constructor(private http: HttpClient) {}

  getUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(`${this.apiUrl}/users`);
  }

  getStats(): Observable<AdminStats> {
    return this.http.get<AdminStats>(`${this.apiUrl}/stats`);
  }

  updateUserRole(userId: number, role: string): Observable<AdminUser[]> {
    return this.http.put<AdminUser[]>(`${this.apiUrl}/users/${userId}/role`, { role });
  }

  getProjects(): Observable<AdminProject[]> {
    return this.http.get<AdminProject[]>(`${this.apiUrl}/projects`);
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/projects/${id}`);
  }
}