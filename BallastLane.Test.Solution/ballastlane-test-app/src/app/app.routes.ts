import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'categories',
    loadChildren: () =>
      import('../features/categories/categories.module').then(m => m.CategoriesModule),
  },
  {
    path: 'products',
    loadChildren: () =>
      import('../features/products/products.module').then(m => m.ProductsModule),
  },
  { path: '', redirectTo: 'categories', pathMatch: 'full' },
  { path: '**', redirectTo: 'categories' },
];
