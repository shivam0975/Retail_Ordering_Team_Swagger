import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';
import { ApiResponse } from '../models/api-response';
import {
  AdminBrand,
  AdminBrandUpsert,
  AdminCategory,
  AdminCategoryUpsert,
  AdminCoupon,
  AdminCouponUpsert,
  AdminDashboard,
  AdminOrder,
  AdminOrderStatusUpdate,
  AdminProduct,
  AdminProductUpsert
} from '../models/admin-model';
import { SessionState } from '../state/session-state';

@Injectable({ providedIn: 'root' })
export class AdminApi {
  constructor(
    private readonly http: HttpClient,
    private readonly sessionState: SessionState
  ) {}

  getDashboard(): Observable<ApiResponse<AdminDashboard>> {
    return this.http.get<ApiResponse<AdminDashboard>>(`${API_BASE_URL}/admin/dashboard`, { headers: this.adminHeaders() });
  }

  getProducts(): Observable<ApiResponse<AdminProduct[]>> {
    return this.http.get<ApiResponse<AdminProduct[]>>(`${API_BASE_URL}/admin/products`, { headers: this.adminHeaders() });
  }

  createProduct(payload: AdminProductUpsert): Observable<ApiResponse<AdminProduct>> {
    return this.http.post<ApiResponse<AdminProduct>>(`${API_BASE_URL}/admin/products`, payload, { headers: this.adminHeaders() });
  }

  updateProduct(productId: number, payload: AdminProductUpsert): Observable<ApiResponse<AdminProduct>> {
    return this.http.put<ApiResponse<AdminProduct>>(`${API_BASE_URL}/admin/products/${productId}`, payload, { headers: this.adminHeaders() });
  }

  deleteProduct(productId: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${API_BASE_URL}/admin/products/${productId}`, { headers: this.adminHeaders() });
  }

  getCategories(): Observable<ApiResponse<AdminCategory[]>> {
    return this.http.get<ApiResponse<AdminCategory[]>>(`${API_BASE_URL}/admin/categories`, { headers: this.adminHeaders() });
  }

  createCategory(payload: AdminCategoryUpsert): Observable<ApiResponse<AdminCategory>> {
    return this.http.post<ApiResponse<AdminCategory>>(`${API_BASE_URL}/admin/categories`, payload, { headers: this.adminHeaders() });
  }

  updateCategory(categoryId: number, payload: AdminCategoryUpsert): Observable<ApiResponse<AdminCategory>> {
    return this.http.put<ApiResponse<AdminCategory>>(`${API_BASE_URL}/admin/categories/${categoryId}`, payload, { headers: this.adminHeaders() });
  }

  deleteCategory(categoryId: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${API_BASE_URL}/admin/categories/${categoryId}`, { headers: this.adminHeaders() });
  }

  getBrands(): Observable<ApiResponse<AdminBrand[]>> {
    return this.http.get<ApiResponse<AdminBrand[]>>(`${API_BASE_URL}/admin/brands`, { headers: this.adminHeaders() });
  }

  createBrand(payload: AdminBrandUpsert): Observable<ApiResponse<AdminBrand>> {
    return this.http.post<ApiResponse<AdminBrand>>(`${API_BASE_URL}/admin/brands`, payload, { headers: this.adminHeaders() });
  }

  updateBrand(brandId: number, payload: AdminBrandUpsert): Observable<ApiResponse<AdminBrand>> {
    return this.http.put<ApiResponse<AdminBrand>>(`${API_BASE_URL}/admin/brands/${brandId}`, payload, { headers: this.adminHeaders() });
  }

  deleteBrand(brandId: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${API_BASE_URL}/admin/brands/${brandId}`, { headers: this.adminHeaders() });
  }

  getCoupons(): Observable<ApiResponse<AdminCoupon[]>> {
    return this.http.get<ApiResponse<AdminCoupon[]>>(`${API_BASE_URL}/admin/coupons`, { headers: this.adminHeaders() });
  }

  createCoupon(payload: AdminCouponUpsert): Observable<ApiResponse<AdminCoupon>> {
    return this.http.post<ApiResponse<AdminCoupon>>(`${API_BASE_URL}/admin/coupons`, payload, { headers: this.adminHeaders() });
  }

  updateCoupon(couponId: number, payload: AdminCouponUpsert): Observable<ApiResponse<AdminCoupon>> {
    return this.http.put<ApiResponse<AdminCoupon>>(`${API_BASE_URL}/admin/coupons/${couponId}`, payload, { headers: this.adminHeaders() });
  }

  deleteCoupon(couponId: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${API_BASE_URL}/admin/coupons/${couponId}`, { headers: this.adminHeaders() });
  }

  getOrders(): Observable<ApiResponse<AdminOrder[]>> {
    return this.http.get<ApiResponse<AdminOrder[]>>(`${API_BASE_URL}/admin/orders`, { headers: this.adminHeaders() });
  }

  updateOrderStatus(orderId: number, payload: AdminOrderStatusUpdate): Observable<ApiResponse<AdminOrder>> {
    return this.http.put<ApiResponse<AdminOrder>>(`${API_BASE_URL}/admin/orders/${orderId}/status`, payload, { headers: this.adminHeaders() });
  }

  private adminHeaders(): HttpHeaders {
    const user = this.sessionState.user();
    return new HttpHeaders({
      'X-User-Id': String(user?.userId ?? 0)
    });
  }
}
