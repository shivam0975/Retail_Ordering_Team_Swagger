export interface Product {
  id: number;
  name: string;
  categoryId: number | null;
  categoryName: string;
  brandId: number | null;
  brandName: string;
  price: number;
  imageUrl: string | null;
  isAvailable: boolean;
  stock: number;
}
