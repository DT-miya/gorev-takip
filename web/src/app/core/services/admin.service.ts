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

export interface AdminUser {
  adminId: number;
  fullName: string;
  email: string;
  role: string;
}

export interface AdminProject {
  id: number;
  name: string;
  description?: string;
  createdAt: string;
  ownerId: number;
  ownerName: string;
  ownerEmail: string;
  columnCount: number;
  taskCount: number;
  memberCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/admin`;


  constructor(private http: HttpClient) {}

  getUsers(): Observable<AdminUser[]> {

    return this.http.get<AdminUser[]>(`${this.apiUrl}/admin/users`);
  }

  getStats(): Observable<AdminStats> {
    return this.http.get<AdminStats>(`${this.apiUrl}/admin/stats`);
  }

  updateUserRole(userId: number, role: string): Observable<AdminUser[]> {
  return this.http.put<AdminUser[]>(`${this.apiUrl}/admin/users/${userId}/role`, { role });

    return this.http.get<AdminUser[]>(`${this.apiUrl}/users`);
  }

  getProjects(): Observable<AdminProject[]> {
    return this.http.get<AdminProject[]>(`${this.apiUrl}/projects`);
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/projects/${id}`);

  }
}