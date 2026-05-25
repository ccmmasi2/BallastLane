import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { environment } from '../../../../environments/environment';
import { CustomerService } from './customer.service';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Customer } from '../models/customer.model';
import { CreateCustomer } from '../models/create-customer.model';
import { UpdateCustomer } from '../models/update-customer.model';

describe('CustomerService', () => {
  let service: CustomerService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/customer`;

  const mockCustomer: Customer = {
    id: 1,
    fullName: 'John Doe',
    documentNumber: '123456789',
    email: 'john.doe@example.com',
    phone: '+1-555-0100',
    address: '123 Main St, Springfield',
  };

  const mockCustomers: Customer[] = [mockCustomer];

  function apiResponse<T>(data: T): ApiResponse<T> {
    return { success: true, message: 'OK', data };
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CustomerService],
    });

    service = TestBed.inject(CustomerService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAll', () => {
    it('should GET all customers and return data array', () => {
      service.getAll().subscribe(customers => {
        expect(customers).toEqual(mockCustomers);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockCustomers));
    });
  });

  describe('getById', () => {
    it('should GET a customer by id and return data', () => {
      service.getById(1).subscribe(customer => {
        expect(customer).toEqual(mockCustomer);
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockCustomer));
    });
  });

  describe('create', () => {
    it('should POST a new customer and return the created id', () => {
      const payload: CreateCustomer = {
        fullName: 'John Doe',
        documentNumber: '123456789',
        email: 'john.doe@example.com',
        phone: '+1-555-0100',
        address: '123 Main St, Springfield',
      };

      service.create(payload).subscribe(id => {
        expect(id).toBe(1);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(payload);
      req.flush(apiResponse(1));
    });
  });

  describe('create with nullable fields', () => {
    it('should POST a customer with only required field and return the created id', () => {
      const payload: CreateCustomer = {
        fullName: 'Jane Smith',
        documentNumber: null,
        email: null,
        phone: null,
        address: null,
      };

      service.create(payload).subscribe(id => {
        expect(id).toBe(2);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(payload);
      req.flush(apiResponse(2));
    });
  });

  describe('update', () => {
    it('should PUT an updated customer and return void', () => {
      const payload: UpdateCustomer = {
        id: 1,
        fullName: 'John Doe Updated',
        documentNumber: '987654321',
        email: 'john.updated@example.com',
        phone: '+1-555-0199',
        address: '456 Oak Ave, Springfield',
      };

      service.update(1, payload).subscribe(result => {
        expect(result).toBeUndefined();
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(payload);
      req.flush(apiResponse(null));
    });
  });

  describe('delete', () => {
    it('should DELETE a customer by id and return void', () => {
      service.delete(1).subscribe(result => {
        expect(result).toBeUndefined();
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(apiResponse(null));
    });
  });
});
