import { Project, CreateProjectRequest, Member, RemoveMemberResponse } from '@models/project.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

export type { Project, CreateProjectRequest, Member };

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private apiUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  getMyProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.apiUrl}/Projects`);
  }

  createProject(data: CreateProjectRequest): Observable<Project> {
    return this.http.post<Project>(`${this.apiUrl}/Projects`, data);
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Projects/${id}`);
  }

  getMembers(projectId: number): Observable<Member[]> {
    return this.http.get<Member[]>(`${this.apiUrl}/Projects/${projectId}/members`);
  }

  addMember(projectId: number, email: string): Observable<Member[]> {
    return this.http.post<Member[]>(`${this.apiUrl}/Projects/${projectId}/members`, { email });
  }

removeMember(projectId: number, memberUserId: number): Observable<RemoveMemberResponse> {
  return this.http.delete<RemoveMemberResponse>(
    `${this.apiUrl}/Projects/${projectId}/members/${memberUserId}`
  );
}

  updateProject(projectId: number, data: CreateProjectRequest): Observable<Project> {
    return this.http.put<Project>(`${this.apiUrl}/Projects/${projectId}`, data);
  }
}