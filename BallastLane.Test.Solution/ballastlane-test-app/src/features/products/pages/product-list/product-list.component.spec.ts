import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';

import { ProductListComponent } from './product-list.component';
import { ProductTableComponent } from '../../components/product-table/product-table.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

const mockProducts: Product[] = [
  { id: 1, categoryId: 1, categoryName: 'Electronics', name: 'Laptop', description: null, price: 999.99 },
];

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let dialogSpy: jasmine.SpyObj<MatDialog>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    productServiceSpy = jasmine.createSpyObj('ProductService', ['getAll', 'delete']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    productServiceSpy.getAll.and.returnValue(of(mockProducts));

    await TestBed.configureTestingModule({
      declarations: [ProductListComponent, ProductTableComponent, LoadingSpinnerComponent],
      imports: [CommonModule, NoopAnimationsModule],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: MatSnackBar, useValue: snackBarSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load products on init', () => {
    expect(productServiceSpy.getAll).toHaveBeenCalled();
    expect(component.products).toEqual(mockProducts);
    expect(component.isLoading).toBeFalse();
  });

  it('should set errorMessage on load failure', () => {
    productServiceSpy.getAll.and.returnValue(throwError(() => ({ error: { message: 'Error' } })));
    component.loadProducts();
    expect(component.errorMessage).toBe('Error');
    expect(component.isLoading).toBeFalse();
  });

  it('should navigate to new product on onCreate', () => {
    component.onCreate();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/products/new']);
  });

  it('should navigate to edit on onEdit', () => {
    component.onEdit(mockProducts[0]);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/products', 1, 'edit']);
  });

  it('should open confirm dialog on onDelete', () => {
    dialogSpy.open.and.returnValue({ afterClosed: () => of(false) } as MatDialogRef<any>);
    component.onDelete(mockProducts[0]);
    expect(dialogSpy.open).toHaveBeenCalled();
  });

  it('should delete product when dialog confirmed', () => {
    productServiceSpy.delete.and.returnValue(of(undefined));
    dialogSpy.open.and.returnValue({ afterClosed: () => of(true) } as MatDialogRef<any>);
    component.onDelete(mockProducts[0]);
    expect(productServiceSpy.delete).toHaveBeenCalledWith(1);
  });
});
