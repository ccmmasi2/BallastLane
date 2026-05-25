import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { InvoiceService } from '../../services/invoice.service';
import { Invoice } from '../../models/invoice.model';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-invoice-list',
  standalone: false,
  templateUrl: './invoice-list.component.html',
  styleUrls: ['./invoice-list.component.scss'],
})
export class InvoiceListComponent implements OnInit, OnDestroy {
  invoices: Invoice[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly invoiceService: InvoiceService,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loadInvoices();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadInvoices(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.invoiceService
      .getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: data => {
          this.invoices = data;
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load invoices.';
          this.isLoading = false;
        },
      });
  }

  onCreate(): void {
    this.router.navigate(['/invoices/new']);
  }

  onView(invoice: Invoice): void {
    this.router.navigate(['/invoices', invoice.id]);
  }

  onDelete(invoice: Invoice): void {
    const data: ConfirmDialogData = {
      title: 'Delete Invoice',
      message: `Are you sure you want to delete invoice #${invoice.id} for "${invoice.customerFullName}"? This action cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data, width: '420px' })
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((confirmed: boolean) => {
        if (confirmed) {
          this.deleteInvoice(invoice.id);
        }
      });
  }

  private deleteInvoice(id: number): void {
    this.invoiceService
      .delete(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Invoice deleted successfully.', 'Close', { duration: 3000 });
          this.loadInvoices();
        },
        error: err => {
          this.snackBar.open(
            err?.error?.message ?? 'Failed to delete invoice.',
            'Close',
            { duration: 4000 },
          );
        },
      });
  }
}
