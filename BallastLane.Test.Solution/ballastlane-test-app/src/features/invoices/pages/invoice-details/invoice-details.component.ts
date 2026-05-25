import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { InvoiceService } from '../../services/invoice.service';
import { Invoice } from '../../models/invoice.model';
import { InvoiceDetail } from '../../models/invoice-detail.model';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-invoice-details',
  standalone: false,
  templateUrl: './invoice-details.component.html',
  styleUrls: ['./invoice-details.component.scss'],
})
export class InvoiceDetailsComponent implements OnInit, OnDestroy {
  invoice: Invoice | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  readonly detailColumns: string[] = [
    'productName',
    'categoryName',
    'unitPrice',
    'quantity',
    'subtotal',
  ];

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly invoiceService: InvoiceService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadInvoice(id);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadInvoice(id: number): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.invoiceService
      .getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: invoice => {
          this.invoice = invoice;
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load invoice.';
          this.isLoading = false;
        },
      });
  }

  onDelete(): void {
    if (!this.invoice) return;

    const data: ConfirmDialogData = {
      title: 'Delete Invoice',
      message: `Are you sure you want to delete invoice #${this.invoice.id}? This action cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data, width: '420px' })
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((confirmed: boolean) => {
        if (confirmed && this.invoice) {
          this.deleteInvoice(this.invoice.id);
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
          this.router.navigate(['/invoices']);
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

  onBack(): void {
    this.router.navigate(['/invoices']);
  }

  getDetails(): InvoiceDetail[] {
    return this.invoice?.details ?? [];
  }
}
