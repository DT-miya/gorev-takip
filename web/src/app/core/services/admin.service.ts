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

  getUsers(searchEmail: string = '', searchName: string = '', page: number = 1, pageSize: number = 20): Observable<PagedResult<AdminUser>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (searchEmail.trim()) {
      params = params.set('searchEmail', searchEmail.trim());
    }

    if (searchName.trim()) {
      params = params.set('searchName', searchName.trim());
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
    const options = limit ? { params: { limit } } : undefined;
    return this.http.get<AdminProject[]>(`${this.apiUrl}/projects`, options);
  }

  getProjectsPage(params: {
    page: number;
    pageSize: number;
    search?: string;
    minColumns?: number | null;
    minTasks?: number | null;
    minMembers?: number | null;
    showArchived?: boolean;
  }): Observable<PagedResult<AdminProject>> {
    let httpParams = new HttpParams()
      .set('page', params.page)
      .set('pageSize', params.pageSize);

    if (params.search) {
      httpParams = httpParams.set('search', params.search);
    }

    if (params.minColumns !== null && params.minColumns !== undefined) {
      httpParams = httpParams.set('minColumns', params.minColumns);
    }

    if (params.minTasks !== null && params.minTasks !== undefined) {
      httpParams = httpParams.set('minTasks', params.minTasks);
    }

    if (params.minMembers !== null && params.minMembers !== undefined) {
      httpParams = httpParams.set('minMembers', params.minMembers);
    }

    httpParams = httpParams.set('showArchived', params.showArchived ?? false);

    return this.http.get<PagedResult<AdminProject>>(`${this.apiUrl}/projects/paged`, {
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

  getLogs(searchEmail: string, searchAction: string, searchDescription: string, page: number = 1, pageSize: number = 20): Observable<PagedResult<ActivityLog>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (searchEmail.trim()) {
      params = params.set('searchUserMail', searchEmail.trim());
    }
    if (searchAction.trim()) {
      params = params.set('searchAction', searchAction.trim());
    }
    if (searchDescription.trim()) {
      params = params.set('searchDescription', searchDescription.trim());
    }

    return this.http.get<PagedResult<ActivityLog>>(`${this.apiUrl}/logs`, { params });
  }
}