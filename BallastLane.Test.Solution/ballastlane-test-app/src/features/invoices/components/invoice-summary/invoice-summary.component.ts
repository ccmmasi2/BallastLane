import { Component, Input } from '@angular/core';
import { Invoice } from '../../models/invoice.model';

@Component({
  selector: 'app-invoice-summary',
  standalone: false,
  templateUrl: './invoice-summary.component.html',
  styleUrls: ['./invoice-summary.component.scss'],
})
export class InvoiceSummaryComponent {
  @Input() invoice!: Invoice;
}
