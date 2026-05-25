import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { InvoiceProductsSelectorComponent } from './invoice-products-selector.component';
import { Product } from '../../../products/models/product.model';

const mockProducts: Product[] = [
  { id: 1, categoryId: 1, categoryName: 'Electronics', name: 'Laptop', description: null, price: 999.99 },
  { id: 2, categoryId: 1, categoryName: 'Electronics', name: 'Mouse', description: null, price: 29.99 },
];

describe('InvoiceProductsSelectorComponent', () => {
  let component: InvoiceProductsSelectorComponent;
  let fixture: ComponentFixture<InvoiceProductsSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InvoiceProductsSelectorComponent],
      imports: [
        CommonModule,
        NoopAnimationsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatSelectModule,
        MatIconModule,
        MatButtonModule,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceProductsSelectorComponent);
    component = fixture.componentInstance;
    component.products = mockProducts;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit addProduct with correct product and reset control', () => {
    spyOn(component.addProduct, 'emit');
    component.productCtrl.setValue(1);
    component.onAdd();
    expect(component.addProduct.emit).toHaveBeenCalledWith(mockProducts[0]);
    expect(component.productCtrl.value).toBeNull();
  });

  it('should not emit when no product selected', () => {
    spyOn(component.addProduct, 'emit');
    component.productCtrl.reset();
    component.onAdd();
    expect(component.addProduct.emit).not.toHaveBeenCalled();
  });
});
