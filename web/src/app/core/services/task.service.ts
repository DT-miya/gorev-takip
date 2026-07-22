import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

export interface TaskItem {
  id: number;
  columnId: number;
  title: string;
  description?: string;
  priority: string;
  order: number;
  assigneeId?: number;
  assigneeName?: string;
}

export interface BoardColumn {
  id: number;
  title: string;
  order: number;
  tasks: TaskItem[];
}

export interface BoardFullResponse {
  projectId: number;
  projectName: string;
  columns: BoardColumn[];
}

export interface CreateTaskRequest {
  columnId: number;
  title: string;
  description?: string;
  priority?: string;
}

export interface MoveTaskRequest {
  targetColumnId: number;
  newOrder: number;
}


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



}