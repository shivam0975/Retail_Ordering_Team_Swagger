import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminBrand, AdminCategory, AdminProduct, AdminProductUpsert } from '../../../models/admin-model';
import { AdminApi } from '../../../services/admin-api';
import { AlertState } from '../../../state/alert-state';

@Component({
  selector: 'app-admin-products-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products-page.html'
})
export class AdminProductsPage implements OnInit {
  readonly products = signal<AdminProduct[]>([]);
  readonly categories = signal<AdminCategory[]>([]);
  readonly brands = signal<AdminBrand[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly editingProductId = signal<number | null>(null);

  form: AdminProductUpsert = {
    name: '',
    categoryId: null,
    brandId: null,
    price: 0,
    imageUrl: null,
    isAvailable: true,
    stock: 0
  };

  constructor(
    private readonly adminApi: AdminApi,
    private readonly alertState: AlertState
  ) {}

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.loading.set(true);
    this.adminApi.getProducts().subscribe({
      next: (response) => {
        if (!response.success) {
          this.loading.set(false);
          this.alertState.error(response.message || 'Failed to load products.');
          return;
        }

        this.products.set(response.data);
        this.loadCatalog();
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Failed to load products.');
      }
    });
  }

  private loadCatalog(): void {
    this.adminApi.getCategories().subscribe({
      next: (categoriesResponse) => {
        if (categoriesResponse.success) {
          this.categories.set(categoriesResponse.data);
        }

        this.adminApi.getBrands().subscribe({
          next: (brandsResponse) => {
            this.loading.set(false);
            if (brandsResponse.success) {
              this.brands.set(brandsResponse.data);
            }
          },
          error: () => {
            this.loading.set(false);
          }
        });
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  edit(product: AdminProduct): void {
    this.editingProductId.set(product.id);
    this.form = {
      name: product.name,
      categoryId: product.categoryId,
      brandId: product.brandId,
      price: product.price,
      imageUrl: product.imageUrl,
      isAvailable: product.isAvailable,
      stock: product.stock
    };
  }

  resetForm(): void {
    this.editingProductId.set(null);
    this.form = {
      name: '',
      categoryId: null,
      brandId: null,
      price: 0,
      imageUrl: null,
      isAvailable: true,
      stock: 0
    };
  }

  save(): void {
    if (!this.form.name.trim()) {
      this.alertState.error('Product name is required.');
      return;
    }

    this.saving.set(true);

    const payload: AdminProductUpsert = {
      ...this.form,
      name: this.form.name.trim(),
      imageUrl: this.form.imageUrl?.trim() || null
    };

    const request$ = this.editingProductId() !== null
      ? this.adminApi.updateProduct(this.editingProductId()!, payload)
      : this.adminApi.createProduct(payload);

    request$.subscribe({
      next: (response) => {
        this.saving.set(false);
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to save product.');
          return;
        }

        this.alertState.success(response.message || 'Product saved successfully.');
        this.resetForm();
        this.loadAll();
      },
      error: (error: HttpErrorResponse) => {
        this.saving.set(false);
        this.alertState.error(error.error?.message || 'Failed to save product.');
      }
    });
  }

  delete(productId: number): void {
    this.adminApi.deleteProduct(productId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to delete product.');
          return;
        }

        this.alertState.success(response.message || 'Product deleted successfully.');
        this.loadAll();
      },
      error: (error: HttpErrorResponse) => {
        this.alertState.error(error.error?.message || 'Failed to delete product.');
      }
    });
  }
}
