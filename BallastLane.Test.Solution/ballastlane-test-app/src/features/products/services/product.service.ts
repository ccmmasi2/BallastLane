import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Product } from '../models/product.model';
import { CreateProduct } from '../models/create-product.model';
import { UpdateProduct } from '../models/update-product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly apiUrl = `${environment.apiUrl}/product`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http
      .get<ApiResponse<Product[]>>(this.apiUrl)
      .pipe(map(response => response.data));
  }

  getById(id: number): Observable<Product> {
    return this.http
      .get<ApiResponse<Product>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  getByCategory(categoryId: number): Observable<Product[]> {
    return this.http
      .get<ApiResponse<Product[]>>(`${this.apiUrl}/category/${categoryId}`)
      .pipe(map(response => response.data));
  }

  create(payload: CreateProduct): Observable<number> {
    return this.http
      .post<ApiResponse<number>>(this.apiUrl, payload)
      .pipe(map(response => response.data));
  }

  update(id: number, payload: UpdateProduct): Observable<void> {
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
