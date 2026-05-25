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
  {
    path: 'customers',
    loadChildren: () =>
      import('../features/customers/customers.module').then(m => m.CustomersModule),
  },
  {
    path: 'invoices',
    loadChildren: () =>
      import('../features/invoices/invoices.module').then(m => m.InvoicesModule),
  },
  { path: '', redirectTo: 'categories', pathMatch: 'full' },
  { path: '**', redirectTo: 'categories' },
];
