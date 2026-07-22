import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

export interface ReorderColumnsRequest {
  projectId: number;
  orderedColumnIds: number[]; // 👈 Backend'in beklediği dizi formatı
}

@Injectable({
  providedIn: 'root'
})
export class ColumnService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  reorderColumns(request: ReorderColumnsRequest): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/Columns/reorder`, request);
  }
}