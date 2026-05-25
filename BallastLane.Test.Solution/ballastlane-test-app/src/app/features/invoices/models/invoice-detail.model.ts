export interface InvoiceDetail {
  id: number;
  productId: number;
  productName: string;
  categoryName: string | null;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}
