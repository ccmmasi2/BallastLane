import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-category-form',
  standalone: false,
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.scss'],
})
export class CategoryFormComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  isEditMode = false;
  categoryId: number | null = null;
  isLoading = false;
  isSaving = false;
  errorMessage: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly fb: FormBuilder,
    private readonly categoryService: CategoryService,
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
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
    });
  }

  private resolveMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.categoryId = Number(id);
      this.loadCategory(this.categoryId);
    }
  }

  private loadCategory(id: number): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.categoryService
      .getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: category => {
          this.form.patchValue({
            name: category.name,
            description: category.description ?? '',
          });
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load category.';
          this.isLoading = false;
        },
      });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.isEditMode && this.categoryId !== null) {
      this.updateCategory();
    } else {
      this.createCategory();
    }
  }

  private createCategory(): void {
    this.isSaving = true;

    const payload = {
      name: this.form.value.name as string,
      description: (this.form.value.description as string) || null,
    };

    this.categoryService
      .create(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Category created successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/categories']);
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to create category.';
          this.isSaving = false;
        },
      });
  }

  private updateCategory(): void {
    this.isSaving = true;

    const payload = {
      id: this.categoryId!,
      name: this.form.value.name as string,
      description: (this.form.value.description as string) || null,
    };

    this.categoryService
      .update(this.categoryId!, payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Category updated successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/categories']);
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to update category.';
          this.isSaving = false;
        },
      });
  }

  onCancel(): void {
    this.router.navigate(['/categories']);
  }
}
