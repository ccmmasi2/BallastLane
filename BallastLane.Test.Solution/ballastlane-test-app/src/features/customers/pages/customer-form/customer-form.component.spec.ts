import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';

import { CustomerFormComponent } from './customer-form.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { CustomerService } from '../../services/customer.service';
import { Customer } from '../../models/customer.model';

const mockCustomer: Customer = {
  id: 3,
  fullName: 'John Doe',
  documentNumber: '12345678',
  email: 'john@example.com',
  phone: '+1 555 0100',
  address: '123 Main St',
};

describe('CustomerFormComponent', () => {
  let component: CustomerFormComponent;
  let fixture: ComponentFixture<CustomerFormComponent>;
  let customerServiceSpy: jasmine.SpyObj<CustomerService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  function createComponent(paramId: string | null = null): void {
    TestBed.overrideProvider(ActivatedRoute, {
      useValue: { snapshot: { paramMap: convertToParamMap(paramId ? { id: paramId } : {}) } },
    });
    fixture = TestBed.createComponent(CustomerFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    customerServiceSpy = jasmine.createSpyObj('CustomerService', ['getById', 'create', 'update']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    customerServiceSpy.getById.and.returnValue(of(mockCustomer));

    await TestBed.configureTestingModule({
      declarations: [CustomerFormComponent, LoadingSpinnerComponent],
      imports: [
        CommonModule,
        NoopAnimationsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatCardModule,
        MatIconModule,
        MatProgressSpinnerModule,
      ],
      providers: [
        { provide: CustomerService, useValue: customerServiceSpy },
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

    it('should be in create mode', () => {
      expect(component.isEditMode).toBeFalse();
      expect(component.customerId).toBeNull();
    });

    it('should mark form invalid when submitted empty', () => {
      component.onSubmit();
      expect(component.form.invalid).toBeTrue();
    });

    it('should reject invalid email', () => {
      component.form.patchValue({ fullName: 'John', email: 'not-an-email' });
      component.onSubmit();
      expect(component.form.get('email')?.hasError('email')).toBeTrue();
    });

    it('should call create on valid submit', () => {
      customerServiceSpy.create.and.returnValue(of(1));
      component.form.setValue({
        fullName: 'John Doe',
        documentNumber: '',
        email: 'john@test.com',
        phone: '',
        address: '',
      });
      component.onSubmit();
      expect(customerServiceSpy.create).toHaveBeenCalled();
    });

    it('should navigate to list on cancel', () => {
      component.onCancel();
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/customers']);
    });
  });

  describe('Edit mode', () => {
    beforeEach(() => createComponent('3'));

    it('should be in edit mode', () => {
      expect(component.isEditMode).toBeTrue();
      expect(component.customerId).toBe(3);
    });

    it('should load customer and patch form', () => {
      expect(customerServiceSpy.getById).toHaveBeenCalledWith(3);
      expect(component.form.value.fullName).toBe('John Doe');
      expect(component.form.value.email).toBe('john@example.com');
    });

    it('should call update on valid submit', () => {
      customerServiceSpy.update.and.returnValue(of(undefined));
      component.onSubmit();
      expect(customerServiceSpy.update).toHaveBeenCalledWith(3, jasmine.any(Object));
    });

    it('should set errorMessage on load failure', () => {
      customerServiceSpy.getById.and.returnValue(
        throwError(() => ({ error: { message: 'Not found' } })),
      );
      component['loadCustomer'](3);
      expect(component.errorMessage).toBe('Not found');
    });
  });
});
