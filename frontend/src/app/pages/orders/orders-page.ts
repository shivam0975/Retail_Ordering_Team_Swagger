import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Order } from '../../models/order-model';
import { OrderApi } from '../../services/order-api';
import { AlertState } from '../../state/alert-state';
import { SessionState } from '../../state/session-state';

@Component({
  selector: 'app-orders-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders-page.html'
})
export class OrdersPage implements OnInit {
  readonly orders = signal<Order[]>([]);
  readonly loading = signal(false);

  constructor(
    private readonly orderApi: OrderApi,
    private readonly sessionState: SessionState,
    private readonly alertState: AlertState,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  private loadOrders(): void {
    const user = this.sessionState.user();
    if (!user) {
      this.router.navigateByUrl('/login');
      return;
    }

    this.loading.set(true);

    this.orderApi.getByUser(user.userId).subscribe({
      next: (response) => {
        this.loading.set(false);

        if (!response.success) {
          this.alertState.error(response.message || 'Failed to load order history.');
          return;
        }

        this.orders.set(response.data);
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Failed to load order history.');
      }
    });
  }
}
