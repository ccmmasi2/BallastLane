import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceProductsSelectorComponent } from './invoice-products-selector.component';

describe('InvoiceProductsSelectorComponent', () => {
  let component: InvoiceProductsSelectorComponent;
  let fixture: ComponentFixture<InvoiceProductsSelectorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceProductsSelectorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceProductsSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
