export interface UpdateInvoiceDetail {
  id: number | null;
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface UpdateInvoice {
  id: number;
  customerId: number;
  invoiceDate: string;
  details: UpdateInvoiceDetail[];
}
