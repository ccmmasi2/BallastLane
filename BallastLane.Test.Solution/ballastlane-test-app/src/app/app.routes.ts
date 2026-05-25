import { Routes } from '@angular/router';

import { DashboardComponent } from '../layouts/dashboard/dashboard.component';
import { authGuard } from '../core/guards/auth.guard';

export const routes: Routes = [
  // Public
  {
    path: 'login',
    loadChildren: () =>
      import('../features/auth/auth.module').then(m => m.AuthModule),
  },

  // Protected — dashboard shell wraps all feature routes
  {
    path: '',
    component: DashboardComponent,
    canActivate: [authGuard],
    children: [
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
    ],
  },

  { path: '**', redirectTo: 'login' },
];
