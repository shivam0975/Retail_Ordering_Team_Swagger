import { Product } from './product-model';
import { DashboardSummary } from './dashboard-model';

export interface AdminCategory {
  id: number;
  name: string;
}

export interface AdminBrand {
  id: number;
  name: string;
}

export interface AdminCoupon {
  id: number;
  code: string;
  discount: number;
  expiryDate: string | null;
}

export interface AdminProductUpsert {
  name: string;
  categoryId: number | null;
  brandId: number | null;
  price: number;
  imageUrl: string | null;
  isAvailable: boolean;
  stock: number;
}

export interface AdminCategoryUpsert {
  name: string;
}

export interface AdminBrandUpsert {
  name: string;
}

export interface AdminCouponUpsert {
  code: string;
  discount: number;
  expiryDate: string | null;
}

export interface AdminOrderItem {
  productId: number;
  productName: string;
  quantity: number;
  price: number;
}

export interface AdminOrder {
  orderId: number;
  userId: number;
  userName: string;
  userEmail: string;
  totalAmount: number;
  status: string;
  createdAt: string;
  deliveryAddress: string;
  items: AdminOrderItem[];
}

export interface AdminOrderStatusUpdate {
  status: string;
}

export interface AdminDashboard extends DashboardSummary {}
export interface AdminProduct extends Product {}
