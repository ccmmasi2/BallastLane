import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
  selector: 'app-category-details',
  standalone: false,
  templateUrl: './category-details.component.html',
  styleUrls: ['./category-details.component.scss'],
})
export class CategoryDetailsComponent implements OnInit, OnDestroy {
  category: Category | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly categoryService: CategoryService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadCategory(id);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCategory(id: number): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.categoryService
      .getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: category => {
          this.category = category;
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load category.';
          this.isLoading = false;
        },
      });
  }

  onEdit(): void {
    this.router.navigate(['/categories', this.category!.id, 'edit']);
  }

  onBack(): void {
    this.router.navigate(['/categories']);
  }

  onDelete(): void {
    if (!this.category) return;

    const data: ConfirmDialogData = {
      title: 'Delete Category',
      message: `Are you sure you want to delete "${this.category.name}"? This action cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data, width: '420px' })
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((confirmed: boolean) => {
        if (confirmed && this.category) {
          this.deleteCategory(this.category.id);
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
          this.router.navigate(['/categories']);
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
