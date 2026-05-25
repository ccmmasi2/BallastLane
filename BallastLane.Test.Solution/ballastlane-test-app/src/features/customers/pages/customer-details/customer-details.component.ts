import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
  selector: 'app-customer-details',
  standalone: false,
  templateUrl: './customer-details.component.html',
  styleUrls: ['./customer-details.component.scss'],
})
export class CustomerDetailsComponent implements OnInit, OnDestroy {
  customer: Customer | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly customerService: CustomerService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadCustomer(id);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCustomer(id: number): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.customerService
      .getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: customer => {
          this.customer = customer;
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load customer.';
          this.isLoading = false;
        },
      });
  }

  onEdit(): void {
    this.router.navigate(['/customers', this.customer!.id, 'edit']);
  }

  onBack(): void {
    this.router.navigate(['/customers']);
  }

  onDelete(): void {
    if (!this.customer) return;

    const data: ConfirmDialogData = {
      title: 'Delete Customer',
      message: `Are you sure you want to delete "${this.customer.fullName}"? This action cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data, width: '420px' })
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((confirmed: boolean) => {
        if (confirmed && this.customer) {
          this.deleteCustomer(this.customer.id);
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
          this.router.navigate(['/customers']);
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
