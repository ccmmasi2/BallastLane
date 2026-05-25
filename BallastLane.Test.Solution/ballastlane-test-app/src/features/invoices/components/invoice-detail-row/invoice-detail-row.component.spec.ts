import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceDetailRowComponent } from './invoice-detail-row.component';

describe('InvoiceDetailRowComponent', () => {
  let component: InvoiceDetailRowComponent;
  let fixture: ComponentFixture<InvoiceDetailRowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceDetailRowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceDetailRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
