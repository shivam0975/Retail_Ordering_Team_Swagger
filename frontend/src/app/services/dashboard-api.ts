import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';
import { ApiResponse } from '../models/api-response';
import { DashboardSummary } from '../models/dashboard-model';

@Injectable({ providedIn: 'root' })
export class DashboardApi {
  constructor(private readonly http: HttpClient) {}

  getSummary(): Observable<ApiResponse<DashboardSummary>> {
    return this.http.get<ApiResponse<DashboardSummary>>(`${API_BASE_URL}/dashboard/summary`);
  }
}
