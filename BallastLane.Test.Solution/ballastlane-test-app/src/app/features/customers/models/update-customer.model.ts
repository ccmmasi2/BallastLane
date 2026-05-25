export interface UpdateCustomer {
  id: number;
  fullName: string;
  documentNumber: string | null;
  email: string | null;
  phone: string | null;
  address: string | null;
}
