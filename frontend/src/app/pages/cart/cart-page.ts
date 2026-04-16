import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Cart } from '../../models/cart-model';
import { CartApi } from '../../services/cart-api';
import { OrderApi } from '../../services/order-api';
import { AlertState } from '../../state/alert-state';
import { SessionState } from '../../state/session-state';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cart-page.html'
})
export class CartPage implements OnInit {
  readonly cart = signal<Cart | null>(null);
  readonly loading = signal(false);
  readonly checkoutLoading = signal(false);

  checkoutForm = {
    deliveryAddress: '',
    couponCode: '',
    paymentMethod: 'CASH'
  };

  constructor(
    private readonly cartApi: CartApi,
    private readonly orderApi: OrderApi,
    private readonly sessionState: SessionState,
    private readonly alertState: AlertState,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    const user = this.sessionState.user();
    if (!user) {
      this.router.navigateByUrl('/login');
      return;
    }

    this.loading.set(true);
    this.cartApi.getByUser(user.userId).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to load cart.');
          return;
        }

        this.cart.set(response.data);
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Failed to load cart.');
      }
    });
  }

  updateQuantity(cartItemId: number, quantity: number): void {
    if (quantity <= 0) {
      this.alertState.error('Quantity must be at least 1.');
      return;
    }

    this.cartApi.updateItem(cartItemId, { quantity }).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Unable to update cart item.');
          return;
        }

        this.cart.set(response.data);
        this.alertState.success(response.message || 'Cart updated.');
      },
      error: (error: HttpErrorResponse) => {
        this.alertState.error(error.error?.message || 'Unable to update cart item.');
      }
    });
  }

  removeItem(cartItemId: number): void {
    this.cartApi.removeItem(cartItemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Unable to remove cart item.');
          return;
        }

        this.alertState.success(response.message || 'Item removed from cart.');
        this.loadCart();
      },
      error: (error: HttpErrorResponse) => {
        this.alertState.error(error.error?.message || 'Unable to remove cart item.');
      }
    });
  }

  checkout(): void {
    const user = this.sessionState.user();
    if (!user) {
      this.router.navigateByUrl('/login');
      return;
    }

    if (!this.checkoutForm.deliveryAddress.trim()) {
      this.alertState.error('Delivery address is required.');
      return;
    }

    this.checkoutLoading.set(true);

    this.orderApi.checkout({
      userId: user.userId,
      deliveryAddress: this.checkoutForm.deliveryAddress,
      couponCode: this.checkoutForm.couponCode || undefined,
      paymentMethod: this.checkoutForm.paymentMethod
    }).subscribe({
      next: (response) => {
        this.checkoutLoading.set(false);

        if (!response.success) {
          this.alertState.error(response.message || 'Checkout failed.');
          return;
        }

        this.alertState.success(response.message || 'Order placed successfully.');
        this.checkoutForm.deliveryAddress = '';
        this.checkoutForm.couponCode = '';
        this.loadCart();
        this.router.navigateByUrl('/orders');
      },
      error: (error: HttpErrorResponse) => {
        this.checkoutLoading.set(false);
        this.alertState.error(error.error?.message || 'Checkout failed.');
      }
    });
  }
}
