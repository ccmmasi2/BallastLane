import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConfirmDialogComponent } from './shared/components/confirm-dialog/confirm-dialog.component';
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner.component';
import { ModalComponent } from './shared/components/modal/modal.component';
import { TableComponent } from './shared/components/table/table.component';
import { ProductListComponent } from './features/products/pages/product-list/product-list.component';
import { ProductFormComponent } from './features/products/pages/product-form/product-form.component';
import { ProductCardComponent } from './features/products/components/product-card/product-card.component';
import { ProductTableComponent } from './features/products/components/product-table/product-table.component';
import { CategoryCardComponent } from './features/categories/components/category-card/category-card.component';
import { CategoryTableComponent } from './features/categories/components/category-table/category-table.component';
import { CategoryFormComponent } from './features/categories/pages/category-form/category-form.component';
import { CategoryListComponent } from './features/categories/pages/category-list/category-list.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DashboardComponent } from './layouts/dashboard/dashboard.component';

@NgModule({
  declarations: [
    AppComponent,
    ConfirmDialogComponent,
    LoadingSpinnerComponent,
    ModalComponent,
    TableComponent,
    ProductListComponent,
    ProductFormComponent,
    ProductCardComponent,
    ProductTableComponent,
    CategoryCardComponent,
    CategoryTableComponent,
    CategoryFormComponent,
    CategoryListComponent,
    DashboardComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
