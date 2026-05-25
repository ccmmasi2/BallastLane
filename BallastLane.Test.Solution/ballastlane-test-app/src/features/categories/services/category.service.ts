import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Category } from '../models/category.model';
import { CreateCategory } from '../models/create-category.model';
import { UpdateCategory } from '../models/update-category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly apiUrl = `${environment.apiUrl}/category`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Category[]> {
    return this.http
      .get<ApiResponse<Category[]>>(this.apiUrl)
      .pipe(map(response => response.data));
  }

  getById(id: number): Observable<Category> {
    return this.http
      .get<ApiResponse<Category>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  create(payload: CreateCategory): Observable<number> {
    return this.http
      .post<ApiResponse<number>>(this.apiUrl, payload)
      .pipe(map(response => response.data));
  }

  update(id: number, payload: UpdateCategory): Observable<void> {
    return this.http
      .put<ApiResponse<null>>(`${this.apiUrl}/${id}`, payload)
      .pipe(map(() => undefined));
  }

  delete(id: number): Observable<void> {
    return this.http
      .delete<ApiResponse<null>>(`${this.apiUrl}/${id}`)
      .pipe(map(() => undefined));
  }
}
