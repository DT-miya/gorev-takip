import { BoardFullResponse, BoardColumn, CreateTaskRequest, MoveTaskRequest, TaskItem, UpdateTaskRequest } from '@models/task.service.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

export type { BoardFullResponse, BoardColumn, CreateTaskRequest, MoveTaskRequest, TaskItem, UpdateTaskRequest };

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  // Tek istekte tüm panoyu (kolonlar + görevler) çeker
  getFullBoard(projectId: number): Observable<BoardFullResponse> {
    return this.http.get<BoardFullResponse>(`${this.apiUrl}/Columns/project/${projectId}/full`);
  }

  // Kolon altına hızlı görev ekler
  createTask(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(`${this.apiUrl}/Tasks`, request);
  }

moveTask(taskId: number, request: MoveTaskRequest): Observable<TaskItem> {
  return this.http.post<TaskItem>(`${this.apiUrl}/Tasks/${taskId}/move`, request);
}


updateTask(taskId: number, request: UpdateTaskRequest): Observable<TaskItem> {
  return this.http.put<TaskItem>(`${this.apiUrl}/Tasks/${taskId}`, request);
}

deleteTask(taskId: number): Observable<boolean> {
  return this.http.delete<boolean>(`${this.apiUrl}/Tasks/${taskId}`);
}



}