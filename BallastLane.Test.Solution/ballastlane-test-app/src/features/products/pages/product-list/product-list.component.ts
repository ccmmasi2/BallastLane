import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
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
  selector: 'app-product-list',
  standalone: false,
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent implements OnInit, OnDestroy {
  products: Product[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly productService: ProductService,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.productService
      .getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: data => {
          this.products = data;
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load products.';
          this.isLoading = false;
        },
      });
  }

  onCreate(): void {
    this.router.navigate(['/products/new']);
  }

  onEdit(product: Product): void {
    this.router.navigate(['/products', product.id, 'edit']);
  }

  onDelete(product: Product): void {
    const data: ConfirmDialogData = {
      title: 'Delete Product',
      message: `Are you sure you want to delete "${product.name}"? This action cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data, width: '400px' })
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((confirmed: boolean) => {
        if (confirmed) {
          this.deleteProduct(product.id);
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
          this.loadProducts();
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
