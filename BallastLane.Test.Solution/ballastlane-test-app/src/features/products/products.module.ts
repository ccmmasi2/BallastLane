import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';
import { ProductsRoutingModule } from './products-routing.module';
import { ProductListComponent } from './pages/product-list/product-list.component';
import { ProductFormComponent } from './pages/product-form/product-form.component';
import { ProductDetailsComponent } from './pages/product-details/product-details.component';
import { ProductTableComponent } from './components/product-table/product-table.component';
import { ProductCardComponent } from './components/product-card/product-card.component';

@NgModule({
  declarations: [
    ProductListComponent,
    ProductFormComponent,
    ProductDetailsComponent,
    ProductTableComponent,
    ProductCardComponent,
  ],
  imports: [SharedModule, ReactiveFormsModule, ProductsRoutingModule],
})
export class ProductsModule {}
