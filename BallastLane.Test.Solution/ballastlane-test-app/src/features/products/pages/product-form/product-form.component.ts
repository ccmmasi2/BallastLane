import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../../categories/services/category.service';
import { Category } from '../../../categories/models/category.model';

@Component({
  selector: 'app-product-form',
  standalone: false,
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss'],
})
export class ProductFormComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  isEditMode = false;
  productId: number | null = null;
  isLoading = false;
  isSaving = false;
  isLoadingCategories = false;
  errorMessage: string | null = null;
  categories: Category[] = [];

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly fb: FormBuilder,
    private readonly productService: ProductService,
    private readonly categoryService: CategoryService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.loadCategories();
    this.resolveMode();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private buildForm(): void {
    this.form = this.fb.group({
      categoryId: [null, [Validators.required]],
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      price: [null, [Validators.required, Validators.min(0.01)]],
    });
  }

  private loadCategories(): void {
    this.isLoadingCategories = true;

    this.categoryService
      .getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: data => {
          this.categories = data;
          this.isLoadingCategories = false;
        },
        error: () => {
          this.isLoadingCategories = false;
        },
      });
  }

  private resolveMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.productId = Number(id);
      this.loadProduct(this.productId);
    }
  }

  private loadProduct(id: number): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.productService
      .getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: product => {
          this.form.patchValue({
            categoryId: product.categoryId,
            name: product.name,
            description: product.description ?? '',
            price: product.price,
          });
          this.isLoading = false;
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to load product.';
          this.isLoading = false;
        },
      });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.isEditMode && this.productId !== null) {
      this.updateProduct();
    } else {
      this.createProduct();
    }
  }

  private createProduct(): void {
    this.isSaving = true;

    const payload = {
      categoryId: this.form.value.categoryId as number,
      name: this.form.value.name as string,
      description: (this.form.value.description as string) || null,
      price: this.form.value.price as number,
    };

    this.productService
      .create(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Product created successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/products']);
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to create product.';
          this.isSaving = false;
        },
      });
  }

  private updateProduct(): void {
    this.isSaving = true;

    const payload = {
      id: this.productId!,
      categoryId: this.form.value.categoryId as number,
      name: this.form.value.name as string,
      description: (this.form.value.description as string) || null,
      price: this.form.value.price as number,
    };

    this.productService
      .update(this.productId!, payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Product updated successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/products']);
        },
        error: err => {
          this.errorMessage = err?.error?.message ?? 'Failed to update product.';
          this.isSaving = false;
        },
      });
  }

  onCancel(): void {
    this.router.navigate(['/products']);
  }
}
