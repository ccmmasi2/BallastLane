import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';

import { InvoiceDetailsComponent } from './invoice-details.component';
import { InvoiceSummaryComponent } from '../../components/invoice-summary/invoice-summary.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { InvoiceService } from '../../services/invoice.service';
import { Invoice } from '../../models/invoice.model';

const mockInvoice: Invoice = {
  id: 5,
  invoiceDate: '2024-02-10T00:00:00',
  total: 499.99,
  createdByUserId: 1,
  customerId: 1,
  customerFullName: 'John Doe',
  customerDocumentNumber: null,
  customerEmail: null,
  customerPhone: null,
  customerAddress: null,
  details: [
    { id: 1, productId: 1, productName: 'Laptop', categoryName: 'Electronics', unitPrice: 499.99, quantity: 1, subtotal: 499.99 },
  ],
};

describe('InvoiceDetailsComponent', () => {
  let component: InvoiceDetailsComponent;
  let fixture: ComponentFixture<InvoiceDetailsComponent>;
  let invoiceServiceSpy: jasmine.SpyObj<InvoiceService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let dialogSpy: jasmine.SpyObj<MatDialog>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    invoiceServiceSpy = jasmine.createSpyObj('InvoiceService', ['getById', 'delete']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    invoiceServiceSpy.getById.and.returnValue(of(mockInvoice));

    await TestBed.configureTestingModule({
      declarations: [InvoiceDetailsComponent, InvoiceSummaryComponent, LoadingSpinnerComponent],
      imports: [CommonModule, NoopAnimationsModule],
      providers: [
        { provide: InvoiceService, useValue: invoiceServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: MatSnackBar, useValue: snackBarSpy },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap({ id: '5' }) } } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load invoice by id on init', () => {
    expect(invoiceServiceSpy.getById).toHaveBeenCalledWith(5);
    expect(component.invoice).toEqual(mockInvoice);
    expect(component.isLoading).toBeFalse();
  });

  it('should set errorMessage on load failure', () => {
    invoiceServiceSpy.getById.and.returnValue(
      throwError(() => ({ error: { message: 'Not found' } })),
    );
    component['loadInvoice'](5);
    expect(component.errorMessage).toBe('Not found');
  });

  it('should navigate back on onBack', () => {
    component.onBack();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/invoices']);
  });

  it('should open confirm dialog on onDelete', () => {
    dialogSpy.open.and.returnValue({ afterClosed: () => of(false) } as MatDialogRef<any>);
    component.onDelete();
    expect(dialogSpy.open).toHaveBeenCalled();
  });

  it('should delete invoice when confirmed and navigate back', () => {
    invoiceServiceSpy.delete.and.returnValue(of(undefined));
    dialogSpy.open.and.returnValue({ afterClosed: () => of(true) } as MatDialogRef<any>);
    component.onDelete();
    expect(invoiceServiceSpy.delete).toHaveBeenCalledWith(5);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/invoices']);
  });

  it('should return details from getDetails', () => {
    expect(component.getDetails().length).toBe(1);
  });
});
