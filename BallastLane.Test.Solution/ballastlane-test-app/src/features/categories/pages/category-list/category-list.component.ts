import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.model';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-category-list',
  standalone: false,
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.scss'],
})
export class CategoryListComponent implements OnInit, OnDestroy {
  categories: Category[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly categoryService: CategoryService,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.categoryService
      .getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: data => {
          this.categories = data;
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load categories.';
          this.isLoading = false;
        },
      });
  }

  onCreate(): void {
    this.router.navigate(['/categories/new']);
  }

  onEdit(category: Category): void {
    this.router.navigate(['/categories', category.id, 'edit']);
  }

  onDelete(category: Category): void {
    const data: ConfirmDialogData = {
      title: 'Delete Category',
      message: `Are you sure you want to delete "${category.name}"? This action cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel',
    };

    const dialogRef = this.dialog.open(ConfirmDialogComponent, { data, width: '400px' });

    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((confirmed: boolean) => {
        if (confirmed) {
          this.deleteCategory(category.id);
        }
      });
  }

  private deleteCategory(id: number): void {
    this.categoryService
      .delete(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Category deleted successfully.', 'Close', { duration: 3000 });
          this.loadCategories();
        },
        error: err => {
          this.snackBar.open(
            err?.error?.message ?? 'Failed to delete category.',
            'Close',
            { duration: 4000 },
          );
        },
      });
  }
}
