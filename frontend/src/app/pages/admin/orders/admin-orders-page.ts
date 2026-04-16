import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminOrder } from '../../../models/admin-model';
import { AdminApi } from '../../../services/admin-api';
import { AlertState } from '../../../state/alert-state';

@Component({
  selector: 'app-admin-orders-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-orders-page.html'
})
export class AdminOrdersPage implements OnInit {
  readonly orders = signal<AdminOrder[]>([]);
  readonly loading = signal(false);
  readonly statuses = ['PLACED', 'CONFIRMED', 'PREPARING', 'OUT_FOR_DELIVERY', 'DELIVERED', 'CANCELLED'];

  constructor(
    private readonly adminApi: AdminApi,
    private readonly alertState: AlertState
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading.set(true);

    this.adminApi.getOrders().subscribe({
      next: (response) => {
        this.loading.set(false);
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to load orders.');
          return;
        }

        this.orders.set(response.data);
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Failed to load orders.');
      }
    });
  }

  updateStatus(order: AdminOrder, status: string): void {
    this.adminApi.updateOrderStatus(order.orderId, { status }).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to update order status.');
          return;
        }

        this.alertState.success(response.message || 'Order status updated successfully.');
        this.loadOrders();
      },
      error: (error: HttpErrorResponse) => {
        this.alertState.error(error.error?.message || 'Failed to update order status.');
      }
    });
  }
}
