import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';

import { ProductFormComponent } from './product-form.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../../categories/services/category.service';
import { Category } from '../../../categories/models/category.model';
import { Product } from '../../models/product.model';

const mockCategories: Category[] = [
  { id: 1, name: 'Electronics', description: null },
];

const mockProduct: Product = {
  id: 5,
  categoryId: 1,
  categoryName: 'Electronics',
  name: 'Laptop',
  description: 'Great laptop',
  price: 999.99,
};

describe('ProductFormComponent', () => {
  let component: ProductFormComponent;
  let fixture: ComponentFixture<ProductFormComponent>;
  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let categoryServiceSpy: jasmine.SpyObj<CategoryService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  function createComponent(paramId: string | null = null): void {
    TestBed.overrideProvider(ActivatedRoute, {
      useValue: { snapshot: { paramMap: convertToParamMap(paramId ? { id: paramId } : {}) } },
    });
    fixture = TestBed.createComponent(ProductFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    productServiceSpy = jasmine.createSpyObj('ProductService', ['getById', 'create', 'update']);
    categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['getAll']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    categoryServiceSpy.getAll.and.returnValue(of(mockCategories));
    productServiceSpy.getById.and.returnValue(of(mockProduct));

    await TestBed.configureTestingModule({
      declarations: [ProductFormComponent, LoadingSpinnerComponent],
      imports: [
        CommonModule,
        NoopAnimationsModule,
        ReactiveFormsModule,
        MatSelectModule,
        MatFormFieldModule,
        MatInputModule,
        MatCardModule,
        MatProgressSpinnerModule,
      ],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: CategoryService, useValue: categoryServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: MatSnackBar, useValue: snackBarSpy },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap({}) } } },
      ],
    }).compileComponents();
  });

  describe('Create mode', () => {
    beforeEach(() => createComponent());

    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('should load categories on init', () => {
      expect(categoryServiceSpy.getAll).toHaveBeenCalled();
      expect(component.categories).toEqual(mockCategories);
    });

    it('should be in create mode', () => {
      expect(component.isEditMode).toBeFalse();
      expect(component.productId).toBeNull();
    });

    it('should mark form invalid when submitted empty', () => {
      component.onSubmit();
      expect(component.form.invalid).toBeTrue();
    });

    it('should call create on valid submit', () => {
      productServiceSpy.create.and.returnValue(of(1));
      component.form.setValue({ categoryId: 1, name: 'Test Product', description: '', price: 19.99 });
      component.onSubmit();
      expect(productServiceSpy.create).toHaveBeenCalled();
    });

    it('should navigate to list on cancel', () => {
      component.onCancel();
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/products']);
    });
  });

  describe('Edit mode', () => {
    beforeEach(() => createComponent('5'));

    it('should be in edit mode', () => {
      expect(component.isEditMode).toBeTrue();
      expect(component.productId).toBe(5);
    });

    it('should load product and patch form', () => {
      expect(productServiceSpy.getById).toHaveBeenCalledWith(5);
      expect(component.form.value.name).toBe('Laptop');
      expect(component.form.value.price).toBe(999.99);
    });

    it('should call update on valid submit', () => {
      productServiceSpy.update.and.returnValue(of(undefined));
      component.onSubmit();
      expect(productServiceSpy.update).toHaveBeenCalledWith(5, jasmine.any(Object));
    });

    it('should set errorMessage on load failure', () => {
      productServiceSpy.getById.and.returnValue(throwError(() => ({ error: { message: 'Not found' } })));
      component['loadProduct'](5);
      expect(component.errorMessage).toBe('Not found');
    });
  });
});
