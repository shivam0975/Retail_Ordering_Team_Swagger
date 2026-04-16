import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';
import { ApiResponse } from '../models/api-response';
import { CheckoutRequest, Order } from '../models/order-model';

@Injectable({ providedIn: 'root' })
export class OrderApi {
  constructor(private readonly http: HttpClient) {}

  checkout(payload: CheckoutRequest): Observable<ApiResponse<Order>> {
    return this.http.post<ApiResponse<Order>>(`${API_BASE_URL}/orders/checkout`, payload);
  }

  getByUser(userId: number): Observable<ApiResponse<Order[]>> {
    return this.http.get<ApiResponse<Order[]>>(`${API_BASE_URL}/orders/user/${userId}`);
  }
}
