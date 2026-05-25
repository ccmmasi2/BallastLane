import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Invoice } from '../models/invoice.model';
import { CreateInvoice } from '../models/create-invoice.model';

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly apiUrl = `${environment.apiUrl}/invoice`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Invoice[]> {
    return this.http
      .get<ApiResponse<Invoice[]>>(this.apiUrl)
      .pipe(map(response => response.data));
  }

  getById(id: number): Observable<Invoice> {
    return this.http
      .get<ApiResponse<Invoice>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  getByCustomer(customerId: number): Observable<Invoice[]> {
    return this.http
      .get<ApiResponse<Invoice[]>>(`${this.apiUrl}/customer/${customerId}`)
      .pipe(map(response => response.data));
  }

  create(payload: CreateInvoice): Observable<number> {
    return this.http
      .post<ApiResponse<number>>(this.apiUrl, payload)
      .pipe(map(response => response.data));
  }

  delete(id: number): Observable<void> {
    return this.http
      .delete<ApiResponse<null>>(`${this.apiUrl}/${id}`)
      .pipe(map(() => undefined));
  }
}
