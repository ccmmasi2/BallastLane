import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { ProductTableComponent } from './product-table.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { Product } from '../../models/product.model';

const mockProducts: Product[] = [
  { id: 1, categoryId: 1, categoryName: 'Electronics', name: 'Laptop', description: null, price: 999.99 },
  { id: 2, categoryId: 1, categoryName: 'Electronics', name: 'Mouse', description: 'Wireless', price: 29.99 },
];

describe('ProductTableComponent', () => {
  let component: ProductTableComponent;
  let fixture: ComponentFixture<ProductTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProductTableComponent, LoadingSpinnerComponent],
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

    fixture = TestBed.createComponent(ProductTableComponent);
    component = fixture.componentInstance;
    component.products = mockProducts;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render product rows', () => {
    const rows = fixture.nativeElement.querySelectorAll('tr[mat-row]');
    expect(rows.length).toBe(2);
  });

  it('should emit edit event', () => {
    spyOn(component.edit, 'emit');
    component.onEdit(mockProducts[0]);
    expect(component.edit.emit).toHaveBeenCalledWith(mockProducts[0]);
  });

  it('should emit delete event', () => {
    spyOn(component.delete, 'emit');
    component.onDelete(mockProducts[0]);
    expect(component.delete.emit).toHaveBeenCalledWith(mockProducts[0]);
  });

  it('should show spinner when loading', () => {
    component.isLoading = true;
    fixture.detectChanges();
    const spinner = fixture.nativeElement.querySelector('app-loading-spinner');
    expect(spinner).toBeTruthy();
  });
});
