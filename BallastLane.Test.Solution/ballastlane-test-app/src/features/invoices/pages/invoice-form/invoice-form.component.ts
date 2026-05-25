import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { forkJoin, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { InvoiceService } from '../../services/invoice.service';
import { CustomerService } from '../../../customers/services/customer.service';
import { ProductService } from '../../../products/services/product.service';
import { Customer } from '../../../customers/models/customer.model';
import { Product } from '../../../products/models/product.model';
import { CreateInvoice } from '../../models/create-invoice.model';

@Component({
  selector: 'app-invoice-form',
  standalone: false,
  templateUrl: './invoice-form.component.html',
  styleUrls: ['./invoice-form.component.scss'],
})
export class InvoiceFormComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  customers: Customer[] = [];
  products: Product[] = [];
  isLoading = false;
  isSaving = false;
  errorMessage: string | null = null;
  detailsErrorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly fb: FormBuilder,
    private readonly invoiceService: InvoiceService,
    private readonly customerService: CustomerService,
    private readonly productService: ProductService,
    private readonly router: Router,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get details(): FormArray {
    return this.form.get('details') as FormArray;
  }

  asFormGroup(control: AbstractControl): FormGroup {
    return control as FormGroup;
  }

  getSubtotal(index: number): number {
    const group = this.details.at(index) as FormGroup;
    const qty = Number(group.get('quantity')?.value) || 0;
    const price = Number(group.get('unitPrice')?.value) || 0;
    return qty * price;
  }

  getTotal(): number {
    return this.details.controls.reduce((sum, _, i) => sum + this.getSubtotal(i), 0);
  }

  private buildForm(): void {
    this.form = this.fb.group({
      customerId: [null, [Validators.required]],
      invoiceDate: [this.todayString(), [Validators.required]],
      details: this.fb.array([]),
    });
  }

  private todayString(): string {
    return new Date().toISOString().split('T')[0];
  }

  private loadData(): void {
    this.isLoading = true;

    forkJoin({
      customers: this.customerService.getAll(),
      products: this.productService.getAll(),
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ customers, products }) => {
          this.customers = customers;
          this.products = products;
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to load form data. Please refresh the page.';
          this.isLoading = false;
        },
      });
  }

  addDetail(product: Product): void {
    this.detailsErrorMessage = null;
    const group = this.fb.group({
      productId: [product.id, [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [product.price, [Validators.required, Validators.min(0.01)]],
    });
    this.details.push(group);
  }

  removeDetail(index: number): void {
    this.details.removeAt(index);
  }

  onSubmit(): void {
    if (this.details.length === 0) {
      this.detailsErrorMessage = 'Add at least one product before saving.';
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.createInvoice();
  }

  private createInvoice(): void {
    this.isSaving = true;

    const payload: CreateInvoice = {
      customerId: this.form.value.customerId as number,
      invoiceDate: this.form.value.invoiceDate as string,
      details: this.details.controls.map(ctrl => {
        const g = ctrl as FormGroup;
        return {
          productId: g.value.productId as number,
          quantity: g.value.quantity as number,
          unitPrice: g.value.unitPrice as number,
        };
      }),
    };

    this.invoiceService
      .create(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Invoice created successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/invoices']);
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to create invoice.';
          this.isSaving = false;
        },
      });
  }

  onCancel(): void {
    this.router.navigate(['/invoices']);
  }
}
