import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';
import { InvoicesRoutingModule } from './invoices-routing.module';
import { InvoiceListComponent } from './pages/invoice-list/invoice-list.component';
import { InvoiceFormComponent } from './pages/invoice-form/invoice-form.component';
import { InvoiceDetailsComponent } from './pages/invoice-details/invoice-details.component';
import { InvoiceTableComponent } from './components/invoice-table/invoice-table.component';
import { InvoiceSummaryComponent } from './components/invoice-summary/invoice-summary.component';
import { InvoiceDetailRowComponent } from './components/invoice-detail-row/invoice-detail-row.component';
import { InvoiceProductsSelectorComponent } from './components/invoice-products-selector/invoice-products-selector.component';

@NgModule({
  declarations: [
    InvoiceListComponent,
    InvoiceFormComponent,
    InvoiceDetailsComponent,
    InvoiceTableComponent,
    InvoiceSummaryComponent,
    InvoiceDetailRowComponent,
    InvoiceProductsSelectorComponent,
  ],
  imports: [SharedModule, ReactiveFormsModule, InvoicesRoutingModule],
})
export class InvoicesModule {}
