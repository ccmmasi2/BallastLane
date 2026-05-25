export interface UpdateProduct {
  id: number;
  categoryId: number;
  name: string;
  description: string | null;
  price: number;
}
