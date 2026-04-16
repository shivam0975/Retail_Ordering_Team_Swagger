import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Product } from '../../models/product-model';
import { CartApi } from '../../services/cart-api';
import { ProductApi } from '../../services/product-api';
import { AlertState } from '../../state/alert-state';
import { SessionState } from '../../state/session-state';

@Component({
  selector: 'app-menu-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './menu-page.html'
})
export class MenuPage implements OnInit {
  readonly products = signal<Product[]>([]);
  readonly loading = signal(false);
  search = '';
  selectedCategoryId = 0;
  readonly categories = signal<Array<{ id: number; name: string }>>([]);

  constructor(
    private readonly productApi: ProductApi,
    private readonly cartApi: CartApi,
    private readonly sessionState: SessionState,
    private readonly alertState: AlertState,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    if (!this.sessionState.user()) {
      this.router.navigateByUrl('/login');
      return;
    }

    this.loadProducts();
  }

  loadProducts(): void {
    this.loading.set(true);

    const categoryId = this.selectedCategoryId > 0 ? this.selectedCategoryId : undefined;

    this.productApi.getProducts(this.search, categoryId).subscribe({
      next: (response) => {
        this.loading.set(false);

        if (!response.success) {
          this.alertState.error(response.message || 'Failed to load products.');
          return;
        }

        this.products.set(response.data);
        this.categories.set(this.buildCategories(response.data));
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Failed to load products.');
      }
    });
  }

  addToCart(product: Product): void {
    const user = this.sessionState.user();
    if (!user) {
      this.router.navigateByUrl('/login');
      return;
    }

    this.cartApi.addItem({ userId: user.userId, productId: product.id, quantity: 1 }).subscribe({
      next: (response) => {
        if (response.success) {
          this.alertState.success(response.message || `${product.name} added to cart.`);
        } else {
          this.alertState.error(response.message || 'Unable to add item to cart.');
        }
      },
      error: (error: HttpErrorResponse) => {
        this.alertState.error(error.error?.message || 'Unable to add item to cart.');
      }
    });
  }

  private buildCategories(products: Product[]): Array<{ id: number; name: string }> {
    const map = new Map<number, string>();

    products.forEach((product) => {
      if (product.categoryId && product.categoryName) {
        map.set(product.categoryId, product.categoryName);
      }
    });

    return Array.from(map.entries())
      .map(([id, name]) => ({ id, name }))
      .sort((a, b) => a.name.localeCompare(b.name));
  }
}
