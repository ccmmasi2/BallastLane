import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerCardComponent } from './components/customer-card/customer-card.component';
import { CustomerTableComponent } from './components/customer-table/customer-table.component';
import { CustomerListComponent } from './pages/customer-list/customer-list.component';
import { CustomerFormComponent } from './pages/customer-form/customer-form.component';



@NgModule({
  declarations: [CustomerCardComponent, CustomerTableComponent, CustomerListComponent, CustomerFormComponent],
  imports: [
    CommonModule
  ]
})
export class CustomersModule { }
