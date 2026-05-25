import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { Validators } from '@angular/forms';

import { InvoiceDetailRowComponent } from './invoice-detail-row.component';
import { Product } from '../../../products/models/product.model';

const mockProducts: Product[] = [
  { id: 1, categoryId: 1, categoryName: 'Electronics', name: 'Laptop', description: null, price: 999.99 },
  { id: 2, categoryId: 1, categoryName: 'Electronics', name: 'Mouse', description: null, price: 29.99 },
];

describe('InvoiceDetailRowComponent', () => {
  let component: InvoiceDetailRowComponent;
  let fixture: ComponentFixture<InvoiceDetailRowComponent>;
  let fb: FormBuilder;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InvoiceDetailRowComponent],
      imports: [
        CommonModule,
        NoopAnimationsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatIconModule,
        MatButtonModule,
        MatTooltipModule,
      ],
    }).compileComponents();

    fb = TestBed.inject(FormBuilder);
    fixture = TestBed.createComponent(InvoiceDetailRowComponent);
    component = fixture.componentInstance;
    component.products = mockProducts;
    component.index = 0;
    component.detailGroup = fb.group({
      productId: [1, Validators.required],
      quantity: [2, [Validators.required, Validators.min(1)]],
      unitPrice: [999.99, [Validators.required, Validators.min(0.01)]],
    });
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should compute subtotal correctly', () => {
    expect(component.subtotal).toBe(1999.98);
  });

  it('should update unit price when product changes', () => {
    component.onProductChange(2);
    expect(component.detailGroup.get('unitPrice')?.value).toBe(29.99);
  });

  it('should update subtotal after product change', () => {
    component.detailGroup.patchValue({ quantity: 3 });
    component.onProductChange(2);
    expect(component.subtotal).toBeCloseTo(89.97, 2);
  });

  it('should emit remove event with index', () => {
    spyOn(component.remove, 'emit');
    component.onRemove();
    expect(component.remove.emit).toHaveBeenCalledWith(0);
  });
});
