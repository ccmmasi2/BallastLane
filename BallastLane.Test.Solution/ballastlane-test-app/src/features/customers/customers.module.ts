import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';
import { CustomersRoutingModule } from './customers-routing.module';
import { CustomerListComponent } from './pages/customer-list/customer-list.component';
import { CustomerFormComponent } from './pages/customer-form/customer-form.component';
import { CustomerDetailsComponent } from './pages/customer-details/customer-details.component';
import { CustomerTableComponent } from './components/customer-table/customer-table.component';
import { CustomerCardComponent } from './components/customer-card/customer-card.component';

@NgModule({
  declarations: [
    CustomerListComponent,
    CustomerFormComponent,
    CustomerDetailsComponent,
    CustomerTableComponent,
    CustomerCardComponent,
  ],
  imports: [SharedModule, ReactiveFormsModule, CustomersRoutingModule],
})
export class CustomersModule {}
