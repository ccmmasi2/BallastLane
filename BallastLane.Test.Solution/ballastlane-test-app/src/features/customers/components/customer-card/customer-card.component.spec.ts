import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { CustomerCardComponent } from './customer-card.component';
import { Customer } from '../../models/customer.model';

const mockCustomer: Customer = {
  id: 1,
  fullName: 'John Doe',
  documentNumber: '12345678',
  email: 'john@example.com',
  phone: '+1 555 0100',
  address: '123 Main St',
};

describe('CustomerCardComponent', () => {
  let component: CustomerCardComponent;
  let fixture: ComponentFixture<CustomerCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomerCardComponent],
      imports: [
        CommonModule,
        NoopAnimationsModule,
        MatCardModule,
        MatIconModule,
        MatButtonModule,
        MatTooltipModule,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerCardComponent);
    component = fixture.componentInstance;
    component.customer = mockCustomer;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display full name', () => {
    const title = fixture.debugElement.query(By.css('mat-card-title'));
    expect(title.nativeElement.textContent).toContain('John Doe');
  });

  it('should display document number', () => {
    const subtitle = fixture.debugElement.query(By.css('mat-card-subtitle'));
    expect(subtitle.nativeElement.textContent).toContain('12345678');
  });

  it('should emit edit event', () => {
    spyOn(component.edit, 'emit');
    const buttons = fixture.debugElement.queryAll(By.css('button[mat-icon-button]'));
    buttons[0].nativeElement.click();
    expect(component.edit.emit).toHaveBeenCalledWith(mockCustomer);
  });

  it('should emit delete event', () => {
    spyOn(component.delete, 'emit');
    const buttons = fixture.debugElement.queryAll(By.css('button[mat-icon-button]'));
    buttons[1].nativeElement.click();
    expect(component.delete.emit).toHaveBeenCalledWith(mockCustomer);
  });
});
