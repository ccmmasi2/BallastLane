import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-product-details',
  standalone: false,
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class ProductDetailsComponent implements OnInit, OnDestroy {
  product: Product | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly productService: ProductService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadProduct(id);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProduct(id: number): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.productService
      .getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: product => {
          this.product = product;
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load product.';
          this.isLoading = false;
        },
      });
  }

  onEdit(): void {
    this.router.navigate(['/products', this.product!.id, 'edit']);
  }

  onBack(): void {
    this.router.navigate(['/products']);
  }

  onDelete(): void {
    if (!this.product) return;

    const data: ConfirmDialogData = {
      title: 'Delete Product',
      message: `Are you sure you want to delete "${this.product.name}"? This action cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data, width: '420px' })
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((confirmed: boolean) => {
        if (confirmed && this.product) {
          this.deleteProduct(this.product.id);
        }
      });
  }

  private deleteProduct(id: number): void {
    this.productService
      .delete(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Product deleted successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/products']);
        },
        error: err => {
          this.snackBar.open(
            err?.error?.message ?? 'Failed to delete product.',
            'Close',
            { duration: 4000 },
          );
        },
      });
  }
}
