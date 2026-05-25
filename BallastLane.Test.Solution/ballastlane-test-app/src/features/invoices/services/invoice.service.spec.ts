import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { environment } from '../../../../environments/environment';
import { InvoiceService } from './invoice.service';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Invoice } from '../models/invoice.model';
import { CreateInvoice, CreateInvoiceDetail } from '../models/create-invoice.model';

describe('InvoiceService', () => {
  let service: InvoiceService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/invoice`;

  const mockDetail = {
    id: 1,
    productId: 10,
    productName: 'Laptop',
    categoryName: 'Electronics',
    unitPrice: 999.99,
    quantity: 2,
    subtotal: 1999.98,
  };

  const mockInvoice: Invoice = {
    id: 1,
    invoiceDate: '2026-05-25T00:00:00',
    total: 1999.98,
    createdByUserId: 3,
    customerId: 5,
    customerFullName: 'John Doe',
    customerDocumentNumber: '123456789',
    customerEmail: 'john.doe@example.com',
    customerPhone: '+1-555-0100',
    customerAddress: '123 Main St, Springfield',
    details: [mockDetail],
  };

  const mockInvoices: Invoice[] = [mockInvoice];

  function apiResponse<T>(data: T): ApiResponse<T> {
    return { success: true, message: 'OK', data };
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [InvoiceService],
    });

    service = TestBed.inject(InvoiceService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAll', () => {
    it('should GET all invoices and return data array', () => {
      service.getAll().subscribe(invoices => {
        expect(invoices).toEqual(mockInvoices);
        expect(invoices[0].details.length).toBe(1);
        expect(invoices[0].details[0].productName).toBe('Laptop');
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockInvoices));
    });
  });

  describe('getById', () => {
    it('should GET an invoice by id and return full invoice with details', () => {
      service.getById(1).subscribe(invoice => {
        expect(invoice).toEqual(mockInvoice);
        expect(invoice.customerFullName).toBe('John Doe');
        expect(invoice.details[0].subtotal).toBe(1999.98);
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockInvoice));
    });
  });

  describe('getByCustomer', () => {
    it('should GET invoices filtered by customerId', () => {
      service.getByCustomer(5).subscribe(invoices => {
        expect(invoices).toEqual(mockInvoices);
        expect(invoices[0].customerId).toBe(5);
      });

      const req = httpMock.expectOne(`${apiUrl}/customer/5`);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockInvoices));
    });
  });

  describe('create', () => {
    it('should POST a new invoice and return the created id', () => {
      const detail: CreateInvoiceDetail = {
        productId: 10,
        quantity: 2,
        unitPrice: 999.99,
      };

      const payload: CreateInvoice = {
        customerId: 5,
        invoiceDate: '2026-05-25T00:00:00',
        details: [detail],
      };

      service.create(payload).subscribe(id => {
        expect(id).toBe(1);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(payload);
      expect(req.request.body.details.length).toBe(1);
      req.flush(apiResponse(1));
    });

    it('should POST an invoice with multiple detail lines', () => {
      const payload: CreateInvoice = {
        customerId: 5,
        invoiceDate: '2026-05-25T00:00:00',
        details: [
          { productId: 10, quantity: 2, unitPrice: 999.99 },
          { productId: 11, quantity: 1, unitPrice: 49.99 },
        ],
      };

      service.create(payload).subscribe(id => {
        expect(id).toBe(2);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('POST');
      expect(req.request.body.details.length).toBe(2);
      req.flush(apiResponse(2));
    });
  });

  describe('delete', () => {
    it('should DELETE an invoice by id and return void', () => {
      service.delete(1).subscribe(result => {
        expect(result).toBeUndefined();
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(apiResponse(null));
    });
  });
});
