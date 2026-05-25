import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Invoice } from '../../models/invoice.model';

@Component({
  selector: 'app-invoice-table',
  standalone: false,
  templateUrl: './invoice-table.component.html',
  styleUrls: ['./invoice-table.component.scss'],
})
export class InvoiceTableComponent {
  @Input() invoices: Invoice[] = [];
  @Input() isLoading = false;
  @Output() view = new EventEmitter<Invoice>();
  @Output() delete = new EventEmitter<Invoice>();

  readonly displayedColumns: string[] = [
    'id',
    'invoiceDate',
    'customer',
    'detailsCount',
    'total',
    'actions',
  ];

  onView(invoice: Invoice): void {
    this.view.emit(invoice);
  }

  onDelete(invoice: Invoice): void {
    this.delete.emit(invoice);
  }
}
