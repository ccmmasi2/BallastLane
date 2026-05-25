import { InvoiceDetail } from './invoice-detail.model';

export interface Invoice {
  id: number;
  invoiceDate: string;
  total: number;
  createdByUserId: number;
  customerId: number;
  customerFullName: string;
  customerDocumentNumber: string | null;
  customerEmail: string | null;
  customerPhone: string | null;
  customerAddress: string | null;
  details: InvoiceDetail[];
}
