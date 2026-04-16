import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminBrand, AdminCategory, AdminCoupon } from '../../../models/admin-model';
import { AdminApi } from '../../../services/admin-api';
import { AlertState } from '../../../state/alert-state';

@Component({
  selector: 'app-admin-catalog-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-catalog-page.html'
})
export class AdminCatalogPage implements OnInit {
  readonly categories = signal<AdminCategory[]>([]);
  readonly brands = signal<AdminBrand[]>([]);
  readonly coupons = signal<AdminCoupon[]>([]);

  categoryName = '';
  brandName = '';
  couponForm = {
    code: '',
    discount: 0,
    expiryDate: ''
  };

  constructor(
    private readonly adminApi: AdminApi,
    private readonly alertState: AlertState
  ) {}

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.adminApi.getCategories().subscribe({
      next: (response) => {
        if (response.success) {
          this.categories.set(response.data);
        }
      }
    });

    this.adminApi.getBrands().subscribe({
      next: (response) => {
        if (response.success) {
          this.brands.set(response.data);
        }
      }
    });

    this.adminApi.getCoupons().subscribe({
      next: (response) => {
        if (response.success) {
          this.coupons.set(response.data);
        }
      }
    });
  }

  createCategory(): void {
    if (!this.categoryName.trim()) {
      this.alertState.error('Category name is required.');
      return;
    }

    this.adminApi.createCategory({ name: this.categoryName.trim() }).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to create category.');
          return;
        }

        this.alertState.success(response.message || 'Category created successfully.');
        this.categoryName = '';
        this.loadAll();
      },
      error: (error: HttpErrorResponse) => {
        this.alertState.error(error.error?.message || 'Failed to create category.');
      }
    });
  }

  deleteCategory(categoryId: number): void {
    this.adminApi.deleteCategory(categoryId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to delete category.');
          return;
        }

        this.alertState.success(response.message || 'Category deleted successfully.');
        this.loadAll();
      }
    });
  }

  createBrand(): void {
    if (!this.brandName.trim()) {
      this.alertState.error('Brand name is required.');
      return;
    }

    this.adminApi.createBrand({ name: this.brandName.trim() }).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to create brand.');
          return;
        }

        this.alertState.success(response.message || 'Brand created successfully.');
        this.brandName = '';
        this.loadAll();
      },
      error: (error: HttpErrorResponse) => {
        this.alertState.error(error.error?.message || 'Failed to create brand.');
      }
    });
  }

  deleteBrand(brandId: number): void {
    this.adminApi.deleteBrand(brandId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to delete brand.');
          return;
        }

        this.alertState.success(response.message || 'Brand deleted successfully.');
        this.loadAll();
      }
    });
  }

  createCoupon(): void {
    if (!this.couponForm.code.trim() || this.couponForm.discount <= 0) {
      this.alertState.error('Coupon code and valid discount are required.');
      return;
    }

    this.adminApi.createCoupon({
      code: this.couponForm.code.trim(),
      discount: this.couponForm.discount,
      expiryDate: this.couponForm.expiryDate || null
    }).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to create coupon.');
          return;
        }

        this.alertState.success(response.message || 'Coupon created successfully.');
        this.couponForm = { code: '', discount: 0, expiryDate: '' };
        this.loadAll();
      },
      error: (error: HttpErrorResponse) => {
        this.alertState.error(error.error?.message || 'Failed to create coupon.');
      }
    });
  }

  deleteCoupon(couponId: number): void {
    this.adminApi.deleteCoupon(couponId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to delete coupon.');
          return;
        }

        this.alertState.success(response.message || 'Coupon deleted successfully.');
        this.loadAll();
      }
    });
  }
}
