import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';
import { CategoriesRoutingModule } from './categories-routing.module';
import { CategoryListComponent } from './pages/category-list/category-list.component';
import { CategoryFormComponent } from './pages/category-form/category-form.component';
import { CategoryDetailsComponent } from './pages/category-details/category-details.component';
import { CategoryTableComponent } from './components/category-table/category-table.component';
import { CategoryCardComponent } from './components/category-card/category-card.component';

@NgModule({
  declarations: [
    CategoryListComponent,
    CategoryFormComponent,
    CategoryDetailsComponent,
    CategoryTableComponent,
    CategoryCardComponent,
  ],
  imports: [SharedModule, ReactiveFormsModule, CategoriesRoutingModule],
})
export class CategoriesModule {}
