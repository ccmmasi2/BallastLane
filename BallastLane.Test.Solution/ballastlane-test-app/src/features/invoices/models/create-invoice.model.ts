export interface CreateInvoiceDetail {
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface CreateInvoice {
  customerId: number;
  invoiceDate: string;
  details: CreateInvoiceDetail[];
}
