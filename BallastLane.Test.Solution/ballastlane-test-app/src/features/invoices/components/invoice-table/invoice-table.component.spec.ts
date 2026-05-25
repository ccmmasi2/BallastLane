import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { InvoiceTableComponent } from './invoice-table.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { Invoice } from '../../models/invoice.model';

const mockInvoices: Invoice[] = [
  {
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
    details: [{ id: 1, productId: 1, productName: 'Laptop', categoryName: 'Electronics', unitPrice: 150, quantity: 1, subtotal: 150 }],
  },
];

describe('InvoiceTableComponent', () => {
  let component: InvoiceTableComponent;
  let fixture: ComponentFixture<InvoiceTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InvoiceTableComponent, LoadingSpinnerComponent],
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

    fixture = TestBed.createComponent(InvoiceTableComponent);
    component = fixture.componentInstance;
    component.invoices = mockInvoices;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render invoice rows', () => {
    const rows = fixture.nativeElement.querySelectorAll('tr[mat-row]');
    expect(rows.length).toBe(1);
  });

  it('should emit view event', () => {
    spyOn(component.view, 'emit');
    component.onView(mockInvoices[0]);
    expect(component.view.emit).toHaveBeenCalledWith(mockInvoices[0]);
  });

  it('should emit delete event', () => {
    spyOn(component.delete, 'emit');
    component.onDelete(mockInvoices[0]);
    expect(component.delete.emit).toHaveBeenCalledWith(mockInvoices[0]);
  });

  it('should show loading spinner when loading', () => {
    component.isLoading = true;
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('app-loading-spinner')).toBeTruthy();
  });
});
