import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { ProductCardComponent } from './product-card.component';
import { Product } from '../../models/product.model';

const mockProduct: Product = {
  id: 1,
  categoryId: 2,
  categoryName: 'Electronics',
  name: 'Laptop',
  description: 'A powerful laptop',
  price: 999.99,
};

describe('ProductCardComponent', () => {
  let component: ProductCardComponent;
  let fixture: ComponentFixture<ProductCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProductCardComponent],
      imports: [NoopAnimationsModule, MatCardModule, MatIconModule, MatButtonModule, MatTooltipModule],
    }).compileComponents();

    fixture = TestBed.createComponent(ProductCardComponent);
    component = fixture.componentInstance;
    component.product = mockProduct;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display product name', () => {
    const title = fixture.debugElement.query(By.css('mat-card-title'));
    expect(title.nativeElement.textContent).toContain('Laptop');
  });

  it('should display category name', () => {
    const subtitle = fixture.debugElement.query(By.css('mat-card-subtitle'));
    expect(subtitle.nativeElement.textContent).toContain('Electronics');
  });

  it('should emit edit event on edit button click', () => {
    spyOn(component.edit, 'emit');
    const buttons = fixture.debugElement.queryAll(By.css('button[mat-icon-button]'));
    buttons[0].nativeElement.click();
    expect(component.edit.emit).toHaveBeenCalledWith(mockProduct);
  });

  it('should emit delete event on delete button click', () => {
    spyOn(component.delete, 'emit');
    const buttons = fixture.debugElement.queryAll(By.css('button[mat-icon-button]'));
    buttons[1].nativeElement.click();
    expect(component.delete.emit).toHaveBeenCalledWith(mockProduct);
  });
});
