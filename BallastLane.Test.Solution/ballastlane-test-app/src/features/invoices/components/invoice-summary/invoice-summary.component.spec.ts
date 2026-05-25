import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

import { InvoiceSummaryComponent } from './invoice-summary.component';
import { Invoice } from '../../models/invoice.model';

const mockInvoice: Invoice = {
  id: 7,
  invoiceDate: '2024-03-20T00:00:00',
  total: 299.98,
  createdByUserId: 1,
  customerId: 2,
  customerFullName: 'Jane Smith',
  customerDocumentNumber: '87654321',
  customerEmail: 'jane@example.com',
  customerPhone: null,
  customerAddress: null,
  details: [],
};

describe('InvoiceSummaryComponent', () => {
  let component: InvoiceSummaryComponent;
  let fixture: ComponentFixture<InvoiceSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InvoiceSummaryComponent],
      imports: [CommonModule],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceSummaryComponent);
    component = fixture.componentInstance;
    component.invoice = mockInvoice;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display customer full name', () => {
    const name = fixture.debugElement.query(By.css('.name'));
    expect(name.nativeElement.textContent).toContain('Jane Smith');
  });

  it('should display invoice id', () => {
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('7');
  });

  it('should display total amount', () => {
    const total = fixture.debugElement.query(By.css('.total-amount'));
    expect(total.nativeElement.textContent).toContain('299.98');
  });
});
