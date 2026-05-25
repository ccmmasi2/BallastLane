import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CustomerService } from '../../services/customer.service';

@Component({
  selector: 'app-customer-form',
  standalone: false,
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.scss'],
})
export class CustomerFormComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  isEditMode = false;
  customerId: number | null = null;
  isLoading = false;
  isSaving = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly fb: FormBuilder,
    private readonly customerService: CustomerService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.resolveMode();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private buildForm(): void {
    this.form = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(150)]],
      documentNumber: ['', [Validators.maxLength(50)]],
      email: ['', [Validators.email, Validators.maxLength(150)]],
      phone: ['', [Validators.maxLength(20)]],
      address: ['', [Validators.maxLength(300)]],
    });
  }

  private resolveMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.customerId = Number(id);
      this.loadCustomer(this.customerId);
    }
  }

  private loadCustomer(id: number): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.customerService
      .getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: customer => {
          this.form.patchValue({
            fullName: customer.fullName,
            documentNumber: customer.documentNumber ?? '',
            email: customer.email ?? '',
            phone: customer.phone ?? '',
            address: customer.address ?? '',
          });
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load customer.';
          this.isLoading = false;
        },
      });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.isEditMode && this.customerId !== null) {
      this.updateCustomer();
    } else {
      this.createCustomer();
    }
  }

  private createCustomer(): void {
    this.isSaving = true;

    const payload = {
      fullName: this.form.value.fullName as string,
      documentNumber: (this.form.value.documentNumber as string) || null,
      email: (this.form.value.email as string) || null,
      phone: (this.form.value.phone as string) || null,
      address: (this.form.value.address as string) || null,
    };

    this.customerService
      .create(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Customer created successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/customers']);
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to create customer.';
          this.isSaving = false;
        },
      });
  }

  private updateCustomer(): void {
    this.isSaving = true;

    const payload = {
      id: this.customerId!,
      fullName: this.form.value.fullName as string,
      documentNumber: (this.form.value.documentNumber as string) || null,
      email: (this.form.value.email as string) || null,
      phone: (this.form.value.phone as string) || null,
      address: (this.form.value.address as string) || null,
    };

    this.customerService
      .update(this.customerId!, payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Customer updated successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/customers']);
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to update customer.';
          this.isSaving = false;
        },
      });
  }

  onCancel(): void {
    this.router.navigate(['/customers']);
  }
}
