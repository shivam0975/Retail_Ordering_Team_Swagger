import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';
import { ApiResponse } from '../models/api-response';
import { AddCartItemRequest, Cart, UpdateCartItemRequest } from '../models/cart-model';

@Injectable({ providedIn: 'root' })
export class CartApi {
  constructor(private readonly http: HttpClient) {}

  getByUser(userId: number): Observable<ApiResponse<Cart>> {
    return this.http.get<ApiResponse<Cart>>(`${API_BASE_URL}/cart/${userId}`);
  }

  addItem(payload: AddCartItemRequest): Observable<ApiResponse<Cart>> {
    return this.http.post<ApiResponse<Cart>>(`${API_BASE_URL}/cart/items`, payload);
  }

  updateItem(cartItemId: number, payload: UpdateCartItemRequest): Observable<ApiResponse<Cart>> {
    return this.http.put<ApiResponse<Cart>>(`${API_BASE_URL}/cart/items/${cartItemId}`, payload);
  }

  removeItem(cartItemId: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${API_BASE_URL}/cart/items/${cartItemId}`);
  }
}
