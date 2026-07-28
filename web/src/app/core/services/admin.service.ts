import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { AdminUser, AdminStats, AdminProject, PagedResult, ActivityLog} from '../models/admin.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/admin`;

  constructor(private http: HttpClient) {}

  getUsers(search: string = '', page: number = 1, pageSize: number = 20): Observable<PagedResult<AdminUser>> {
  let params = new HttpParams()
    .set('page', page.toString())
    .set('pageSize', pageSize.toString());

  if (search.trim()) {
    params = params.set('search', search.trim());
  }

  return this.http.get<PagedResult<AdminUser>>(`${this.apiUrl}/users`, { params });
}

  getStats(): Observable<AdminStats> {
    return this.http.get<AdminStats>(`${this.apiUrl}/stats`);
  }

updateUserRole(userId: number, role: string): Observable<AdminUser> {
  return this.http.put<AdminUser>(`${this.apiUrl}/users/${userId}/role`, { role });
}

  getProjects(): Observable<AdminProject[]> {
    return this.http.get<AdminProject[]>(`${this.apiUrl}/projects`);
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/projects/${id}`);
  }




getLogs(search: string = '', page: number = 1, pageSize: number = 20): Observable<PagedResult<ActivityLog>> {
  let params = new HttpParams()
    .set('page', page.toString())
    .set('pageSize', pageSize.toString());

  if (search.trim()) {
    params = params.set('search', search.trim());
  }

  return this.http.get<PagedResult<ActivityLog>>(`${this.apiUrl}/logs`, { params });
}

}