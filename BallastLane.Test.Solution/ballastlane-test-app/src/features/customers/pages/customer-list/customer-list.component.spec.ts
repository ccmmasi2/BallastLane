import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';

import { CustomerListComponent } from './customer-list.component';
import { CustomerTableComponent } from '../../components/customer-table/customer-table.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { CustomerService } from '../../services/customer.service';
import { Customer } from '../../models/customer.model';

const mockCustomers: Customer[] = [
  { id: 1, fullName: 'John Doe', documentNumber: '111', email: 'john@test.com', phone: null, address: null },
];

describe('CustomerListComponent', () => {
  let component: CustomerListComponent;
  let fixture: ComponentFixture<CustomerListComponent>;
  let customerServiceSpy: jasmine.SpyObj<CustomerService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let dialogSpy: jasmine.SpyObj<MatDialog>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    customerServiceSpy = jasmine.createSpyObj('CustomerService', ['getAll', 'delete']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    customerServiceSpy.getAll.and.returnValue(of(mockCustomers));

    await TestBed.configureTestingModule({
      declarations: [CustomerListComponent, CustomerTableComponent, LoadingSpinnerComponent],
      imports: [CommonModule, NoopAnimationsModule],
      providers: [
        { provide: CustomerService, useValue: customerServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: MatSnackBar, useValue: snackBarSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load customers on init', () => {
    expect(customerServiceSpy.getAll).toHaveBeenCalled();
    expect(component.customers).toEqual(mockCustomers);
    expect(component.isLoading).toBeFalse();
  });

  it('should set errorMessage on load failure', () => {
    customerServiceSpy.getAll.and.returnValue(
      throwError(() => ({ error: { message: 'Server error' } })),
    );
    component.loadCustomers();
    expect(component.errorMessage).toBe('Server error');
    expect(component.isLoading).toBeFalse();
  });

  it('should navigate to new customer on onCreate', () => {
    component.onCreate();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/customers/new']);
  });

  it('should navigate to edit on onEdit', () => {
    component.onEdit(mockCustomers[0]);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/customers', 1, 'edit']);
  });

  it('should open confirm dialog on onDelete', () => {
    dialogSpy.open.and.returnValue({ afterClosed: () => of(false) } as MatDialogRef<any>);
    component.onDelete(mockCustomers[0]);
    expect(dialogSpy.open).toHaveBeenCalled();
  });

  it('should delete customer when dialog confirmed', () => {
    customerServiceSpy.delete.and.returnValue(of(undefined));
    dialogSpy.open.and.returnValue({ afterClosed: () => of(true) } as MatDialogRef<any>);
    component.onDelete(mockCustomers[0]);
    expect(customerServiceSpy.delete).toHaveBeenCalledWith(1);
  });
});
