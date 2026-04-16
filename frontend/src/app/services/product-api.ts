import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';
import { ApiResponse } from '../models/api-response';
import { Product } from '../models/product-model';

@Injectable({ providedIn: 'root' })
export class ProductApi {
  constructor(private readonly http: HttpClient) {}

  getProducts(search?: string, categoryId?: number): Observable<ApiResponse<Product[]>> {
    let params = new HttpParams();

    if (search?.trim()) {
      params = params.set('search', search.trim());
    }

    if (categoryId && categoryId > 0) {
      params = params.set('categoryId', categoryId);
    }

    return this.http.get<ApiResponse<Product[]>>(`${API_BASE_URL}/products`, { params });
  }
}
