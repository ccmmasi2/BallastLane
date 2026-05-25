import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';

import { InvoiceListComponent } from './invoice-list.component';
import { InvoiceTableComponent } from '../../components/invoice-table/invoice-table.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { InvoiceService } from '../../services/invoice.service';
import { Invoice } from '../../models/invoice.model';

const mockInvoice: Invoice = {
  id: 1,
  invoiceDate: '2024-01-15T00:00:00',
  total: 150.00,
  createdByUserId: 1,
  customerId: 1,
  customerFullName: 'John Doe',
  customerDocumentNumber: null,
  customerEmail: null,
  customerPhone: null,
  customerAddress: null,
  details: [],
};

describe('InvoiceListComponent', () => {
  let component: InvoiceListComponent;
  let fixture: ComponentFixture<InvoiceListComponent>;
  let invoiceServiceSpy: jasmine.SpyObj<InvoiceService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let dialogSpy: jasmine.SpyObj<MatDialog>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    invoiceServiceSpy = jasmine.createSpyObj('InvoiceService', ['getAll', 'delete']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    invoiceServiceSpy.getAll.and.returnValue(of([mockInvoice]));

    await TestBed.configureTestingModule({
      declarations: [InvoiceListComponent, InvoiceTableComponent, LoadingSpinnerComponent],
      imports: [CommonModule, NoopAnimationsModule],
      providers: [
        { provide: InvoiceService, useValue: invoiceServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: MatSnackBar, useValue: snackBarSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load invoices on init', () => {
    expect(invoiceServiceSpy.getAll).toHaveBeenCalled();
    expect(component.invoices.length).toBe(1);
    expect(component.isLoading).toBeFalse();
  });

  it('should set errorMessage on load failure', () => {
    invoiceServiceSpy.getAll.and.returnValue(
      throwError(() => ({ error: { message: 'Server error' } })),
    );
    component.loadInvoices();
    expect(component.errorMessage).toBe('Server error');
  });

  it('should navigate to new invoice on onCreate', () => {
    component.onCreate();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/invoices/new']);
  });

  it('should navigate to detail on onView', () => {
    component.onView(mockInvoice);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/invoices', 1]);
  });

  it('should open confirm dialog on onDelete', () => {
    dialogSpy.open.and.returnValue({ afterClosed: () => of(false) } as MatDialogRef<any>);
    component.onDelete(mockInvoice);
    expect(dialogSpy.open).toHaveBeenCalled();
  });

  it('should delete invoice when dialog confirmed', () => {
    invoiceServiceSpy.delete.and.returnValue(of(undefined));
    dialogSpy.open.and.returnValue({ afterClosed: () => of(true) } as MatDialogRef<any>);
    component.onDelete(mockInvoice);
    expect(invoiceServiceSpy.delete).toHaveBeenCalledWith(1);
  });
});
