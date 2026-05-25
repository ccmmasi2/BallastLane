import { InvoiceDetail } from './invoice-detail.model';

export interface Invoice {
  id: number;
  customerId: number;
  invoiceDate: Date;
  total: number;
  details: InvoiceDetail[];
}