import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { environment } from '../../../../environments/environment';
import { ProductService } from './product.service';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Product } from '../models/product.model';
import { CreateProduct } from '../models/create-product.model';
import { UpdateProduct } from '../models/update-product.model';

describe('ProductService', () => {
  let service: ProductService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/product`;

  const mockProduct: Product = {
    id: 1,
    categoryId: 2,
    categoryName: 'Electronics',
    name: 'Laptop',
    description: 'High-end laptop',
    price: 999.99,
  };

  const mockProducts: Product[] = [mockProduct];

  function apiResponse<T>(data: T): ApiResponse<T> {
    return { success: true, message: 'OK', data };
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProductService],
    });

    service = TestBed.inject(ProductService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAll', () => {
    it('should GET all products and return data array', () => {
      service.getAll().subscribe(products => {
        expect(products).toEqual(mockProducts);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockProducts));
    });
  });

  describe('getById', () => {
    it('should GET a product by id and return data', () => {
      service.getById(1).subscribe(product => {
        expect(product).toEqual(mockProduct);
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockProduct));
    });
  });

  describe('getByCategory', () => {
    it('should GET products filtered by categoryId', () => {
      service.getByCategory(2).subscribe(products => {
        expect(products).toEqual(mockProducts);
      });

      const req = httpMock.expectOne(`${apiUrl}/category/2`);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockProducts));
    });
  });

  describe('create', () => {
    it('should POST a new product and return the created id', () => {
      const payload: CreateProduct = {
        categoryId: 2,
        name: 'Laptop',
        description: 'High-end laptop',
        price: 999.99,
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

  describe('update', () => {
    it('should PUT an updated product and return void', () => {
      const payload: UpdateProduct = {
        id: 1,
        categoryId: 2,
        name: 'Laptop Pro',
        description: 'Updated high-end laptop',
        price: 1199.99,
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
    it('should DELETE a product by id and return void', () => {
      service.delete(1).subscribe(result => {
        expect(result).toBeUndefined();
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(apiResponse(null));
    });
  });
});
