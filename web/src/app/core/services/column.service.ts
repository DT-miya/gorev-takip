import { ReorderColumnsRequest } from '@models/column.service.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

export type { ReorderColumnsRequest };

@Injectable({
  providedIn: 'root'
})
export class ColumnService {
  private apiUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

 // Kolon Ekleme
  createColumn(request: { projectId: number; title: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/Columns`, request);
  }

  // Kolon Adı Güncelleme
  updateColumn(columnId: number, request: { title: string }): Observable<any> {
    return this.http.put(`${this.apiUrl}/Columns/${columnId}`, request);
  }

  // Kolon Silme
  deleteColumn(columnId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Columns/${columnId}`);
  }

  // Kolon Sıralama (Zaten var olan metodun)
  reorderColumns(payload: { projectId: number; orderedColumnIds: number[] }): Observable<any> {
    return this.http.post(`${this.apiUrl}/Columns/reorder`, payload);
  }
}