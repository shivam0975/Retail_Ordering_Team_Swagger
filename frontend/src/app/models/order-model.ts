export interface CheckoutRequest {
  userId: number;
  deliveryAddress: string;
  couponCode?: string;
  paymentMethod: string;
}

export interface OrderItem {
  productId: number;
  productName: string;
  quantity: number;
  price: number;
}

export interface Order {
  orderId: number;
  userId: number;
  totalAmount: number;
  status: string;
  createdAt: string;
  deliveryAddress: string;
  items: OrderItem[];
}
