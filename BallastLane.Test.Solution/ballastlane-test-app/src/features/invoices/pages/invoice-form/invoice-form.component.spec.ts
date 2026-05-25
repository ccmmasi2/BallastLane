import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { forkJoin, of, throwError } from 'rxjs';

import { InvoiceFormComponent } from './invoice-form.component';
import { InvoiceService } from '../../services/invoice.service';
import { CustomerService } from '../../../customers/services/customer.service';
import { ProductService } from '../../../products/services/product.service';
import { Customer } from '../../../customers/models/customer.model';
import { Product } from '../../../products/models/product.model';

const mockCustomers: Customer[] = [
  { id: 1, fullName: 'John Doe', documentNumber: null, email: null, phone: null, address: null },
];

const mockProducts: Product[] = [
  { id: 1, categoryId: 1, categoryName: 'Electronics', name: 'Laptop', description: null, price: 999.99 },
];

describe('InvoiceFormComponent', () => {
  let component: InvoiceFormComponent;
  let fixture: ComponentFixture<InvoiceFormComponent>;
  let invoiceServiceSpy: jasmine.SpyObj<InvoiceService>;
  let customerServiceSpy: jasmine.SpyObj<CustomerService>;
  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    invoiceServiceSpy = jasmine.createSpyObj('InvoiceService', ['create']);
    customerServiceSpy = jasmine.createSpyObj('CustomerService', ['getAll']);
    productServiceSpy = jasmine.createSpyObj('ProductService', ['getAll']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    customerServiceSpy.getAll.and.returnValue(of(mockCustomers));
    productServiceSpy.getAll.and.returnValue(of(mockProducts));

    await TestBed.configureTestingModule({
      declarations: [InvoiceFormComponent],
      imports: [CommonModule, NoopAnimationsModule, ReactiveFormsModule],
      providers: [
        { provide: InvoiceService, useValue: invoiceServiceSpy },
        { provide: CustomerService, useValue: customerServiceSpy },
        { provide: ProductService, useValue: productServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: MatSnackBar, useValue: snackBarSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load customers and products on init', () => {
    expect(customerServiceSpy.getAll).toHaveBeenCalled();
    expect(productServiceSpy.getAll).toHaveBeenCalled();
    expect(component.customers).toEqual(mockCustomers);
    expect(component.products).toEqual(mockProducts);
  });

  it('should start with empty details', () => {
    expect(component.details.length).toBe(0);
  });

  it('should add detail when addDetail is called', () => {
    component.addDetail(mockProducts[0]);
    expect(component.details.length).toBe(1);
    expect(component.details.at(0).value.productId).toBe(1);
    expect(component.details.at(0).value.unitPrice).toBe(999.99);
  });

  it('should remove detail by index', () => {
    component.addDetail(mockProducts[0]);
    component.removeDetail(0);
    expect(component.details.length).toBe(0);
  });

  it('should compute subtotal correctly', () => {
    component.addDetail(mockProducts[0]);
    component.details.at(0).patchValue({ quantity: 2 });
    expect(component.getSubtotal(0)).toBeCloseTo(1999.98, 2);
  });

  it('should compute total across all details', () => {
    component.addDetail(mockProducts[0]);
    component.addDetail(mockProducts[0]);
    component.details.at(0).patchValue({ quantity: 1 });
    component.details.at(1).patchValue({ quantity: 2 });
    expect(component.getTotal()).toBeCloseTo(2999.97, 2);
  });

  it('should set detailsErrorMessage when submitting with no details', () => {
    component.form.patchValue({ customerId: 1, invoiceDate: '2024-01-15' });
    component.onSubmit();
    expect(component.detailsErrorMessage).toBeTruthy();
  });

  it('should call create with correct payload on valid submit', () => {
    invoiceServiceSpy.create.and.returnValue(of(1));
    component.form.patchValue({ customerId: 1, invoiceDate: '2024-01-15' });
    component.addDetail(mockProducts[0]);
    component.onSubmit();
    expect(invoiceServiceSpy.create).toHaveBeenCalledWith(jasmine.objectContaining({
      customerId: 1,
      invoiceDate: '2024-01-15',
    }));
  });

  it('should navigate to invoices list on cancel', () => {
    component.onCancel();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/invoices']);
  });

  it('should set errorMessage on create failure', () => {
    component.form.patchValue({ customerId: 1, invoiceDate: '2024-01-15' });
    component.addDetail(mockProducts[0]);
    invoiceServiceSpy.create.and.returnValue(throwError(() => ({ error: { message: 'Error' } })));
    component.onSubmit();
    expect(component.errorMessage).toBe('Error');
  });
});
