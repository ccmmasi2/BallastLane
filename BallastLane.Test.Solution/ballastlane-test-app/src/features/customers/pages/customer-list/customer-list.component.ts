import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CustomerService } from '../../services/customer.service';
import { Customer } from '../../models/customer.model';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-customer-list',
  standalone: false,
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss'],
})
export class CustomerListComponent implements OnInit, OnDestroy {
  customers: Customer[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly customerService: CustomerService,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loadCustomers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCustomers(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.customerService
      .getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: data => {
          this.customers = data;
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load customers.';
          this.isLoading = false;
        },
      });
  }

  onCreate(): void {
    this.router.navigate(['/customers/new']);
  }

  onEdit(customer: Customer): void {
    this.router.navigate(['/customers', customer.id, 'edit']);
  }

  onDelete(customer: Customer): void {
    const data: ConfirmDialogData = {
      title: 'Delete Customer',
      message: `Are you sure you want to delete "${customer.fullName}"? This action cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data, width: '400px' })
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((confirmed: boolean) => {
        if (confirmed) {
          this.deleteCustomer(customer.id);
        }
      });
  }

  private deleteCustomer(id: number): void {
    this.customerService
      .delete(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Customer deleted successfully.', 'Close', { duration: 3000 });
          this.loadCustomers();
        },
        error: err => {
          this.snackBar.open(
            err?.error?.message ?? 'Failed to delete customer.',
            'Close',
            { duration: 4000 },
          );
        },
      });
  }
}
