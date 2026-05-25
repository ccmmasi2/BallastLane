import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { environment } from '../../../../environments/environment';
import { CategoryService } from './category.service';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Category } from '../models/category.model';
import { CreateCategory } from '../models/create-category.model';
import { UpdateCategory } from '../models/update-category.model';

describe('CategoryService', () => {
  let service: CategoryService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/category`;

  const mockCategory: Category = {
    id: 1,
    name: 'Electronics',
    description: 'Electronic devices and accessories',
  };

  const mockCategories: Category[] = [mockCategory];

  function apiResponse<T>(data: T): ApiResponse<T> {
    return { success: true, message: 'OK', data };
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CategoryService],
    });

    service = TestBed.inject(CategoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAll', () => {
    it('should GET all categories and return data array', () => {
      service.getAll().subscribe(categories => {
        expect(categories).toEqual(mockCategories);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockCategories));
    });
  });

  describe('getById', () => {
    it('should GET a category by id and return data', () => {
      service.getById(1).subscribe(category => {
        expect(category).toEqual(mockCategory);
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(apiResponse(mockCategory));
    });
  });

  describe('create', () => {
    it('should POST a new category and return the created id', () => {
      const payload: CreateCategory = {
        name: 'Electronics',
        description: 'Electronic devices and accessories',
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
    it('should PUT an updated category and return void', () => {
      const payload: UpdateCategory = {
        id: 1,
        name: 'Electronics & Gadgets',
        description: 'Updated description',
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
    it('should DELETE a category by id and return void', () => {
      service.delete(1).subscribe(result => {
        expect(result).toBeUndefined();
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(apiResponse(null));
    });
  });
});
