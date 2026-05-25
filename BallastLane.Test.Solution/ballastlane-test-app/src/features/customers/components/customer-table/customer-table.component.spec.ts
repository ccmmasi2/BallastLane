import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { CustomerTableComponent } from './customer-table.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { Customer } from '../../models/customer.model';

const mockCustomers: Customer[] = [
  { id: 1, fullName: 'John Doe', documentNumber: '111', email: 'john@test.com', phone: '555-0001', address: null },
  { id: 2, fullName: 'Jane Smith', documentNumber: null, email: null, phone: null, address: '456 Oak Ave' },
];

describe('CustomerTableComponent', () => {
  let component: CustomerTableComponent;
  let fixture: ComponentFixture<CustomerTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomerTableComponent, LoadingSpinnerComponent],
      imports: [
        CommonModule,
        NoopAnimationsModule,
        MatTableModule,
        MatIconModule,
        MatButtonModule,
        MatTooltipModule,
        MatProgressSpinnerModule,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerTableComponent);
    component = fixture.componentInstance;
    component.customers = mockCustomers;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render customer rows', () => {
    const rows = fixture.nativeElement.querySelectorAll('tr[mat-row]');
    expect(rows.length).toBe(2);
  });

  it('should emit edit event', () => {
    spyOn(component.edit, 'emit');
    component.onEdit(mockCustomers[0]);
    expect(component.edit.emit).toHaveBeenCalledWith(mockCustomers[0]);
  });

  it('should emit delete event', () => {
    spyOn(component.delete, 'emit');
    component.onDelete(mockCustomers[0]);
    expect(component.delete.emit).toHaveBeenCalledWith(mockCustomers[0]);
  });

  it('should show spinner when loading', () => {
    component.isLoading = true;
    fixture.detectChanges();
    const spinner = fixture.nativeElement.querySelector('app-loading-spinner');
    expect(spinner).toBeTruthy();
  });
});
