export interface CreateProduct {
  categoryId: number;
  name: string;
  description: string | null;
  price: number;
}
