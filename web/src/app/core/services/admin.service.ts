import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { 
  AdminUser, 
  AdminStats, 
  AdminProject, 
  PagedResult, 
  ActivityLog 
} from '../models/admin.model';

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

  getProjects(limit?: number): Observable<AdminProject[]> {
    const options = limit ? { params: new HttpParams().set('limit', limit.toString()) } : undefined;
    return this.http.get<AdminProject[]>(`${this.apiUrl}/projects/all`, options);
  }

  // ⚡ DÜZELTİLEN YER: PagedResult döner, /projects endpoint'ine istek atar
  getProjectsPage(params: {
    page: number;
    pageSize: number;
    projectName?: string;
    ownerSearch?: string;
    minColumns?: number | null;
    minTasks?: number | null;
    minMembers?: number | null;
    showArchived?: boolean;
  }): Observable<PagedResult<AdminProject>> {
    let httpParams = new HttpParams()
      .set('page', params.page.toString())
      .set('pageSize', params.pageSize.toString());

    if (params.projectName && params.projectName.trim()) {
      httpParams = httpParams.set('projectName', params.projectName.trim());
    }

    if (params.ownerSearch && params.ownerSearch.trim()) {
      httpParams = httpParams.set('ownerSearch', params.ownerSearch.trim());
    }

    if (params.minColumns !== null && params.minColumns !== undefined) {
      httpParams = httpParams.set('minColumns', params.minColumns.toString());
    }

    if (params.minTasks !== null && params.minTasks !== undefined) {
      httpParams = httpParams.set('minTasks', params.minTasks.toString());
    }

    if (params.minMembers !== null && params.minMembers !== undefined) {
      httpParams = httpParams.set('minMembers', params.minMembers.toString());
    }

    httpParams = httpParams.set('showArchived', (params.showArchived ?? false).toString());

    return this.http.get<PagedResult<AdminProject>>(`${this.apiUrl}/projects`, {
      params: httpParams
    });
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/projects/${id}`);
  }

  getProjectTasks(projectId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/projects/${projectId}/tasks`);
  }

  getProjectMembers(projectId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/projects/${projectId}/members`);
  }

  archiveProject(projectId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/projects/${projectId}/archive`, {});
  }

  unarchiveProject(projectId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/projects/${projectId}/unarchive`, {});
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