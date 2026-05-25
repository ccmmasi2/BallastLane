import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Customer } from '../models/customer.model';
import { CreateCustomer } from '../models/create-customer.model';
import { UpdateCustomer } from '../models/update-customer.model';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly apiUrl = `${environment.apiUrl}/customer`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Customer[]> {
    return this.http
      .get<ApiResponse<Customer[]>>(this.apiUrl)
      .pipe(map(response => response.data));
  }

  getById(id: number): Observable<Customer> {
    return this.http
      .get<ApiResponse<Customer>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  create(payload: CreateCustomer): Observable<number> {
    return this.http
      .post<ApiResponse<number>>(this.apiUrl, payload)
      .pipe(map(response => response.data));
  }

  update(id: number, payload: UpdateCustomer): Observable<void> {
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
