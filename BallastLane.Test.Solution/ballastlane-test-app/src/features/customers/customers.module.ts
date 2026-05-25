import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { SharedModule } from '../../shared/shared.module';
import { CustomersRoutingModule } from './customers-routing.module';
import { CustomerListComponent } from './pages/customer-list/customer-list.component';
import { CustomerFormComponent } from './pages/customer-form/customer-form.component';
import { CustomerTableComponent } from './components/customer-table/customer-table.component';
import { CustomerCardComponent } from './components/customer-card/customer-card.component';

@NgModule({
  declarations: [
    CustomerListComponent,
    CustomerFormComponent,
    CustomerTableComponent,
    CustomerCardComponent,
  ],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    CustomersRoutingModule,
    MatTableModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatSnackBarModule,
  ],
})
export class CustomersModule {}
