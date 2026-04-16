import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';
import { ApiResponse } from '../models/api-response';
import { AdminRegisterRequest, LoginRequest, RegisterRequest, UserSession } from '../models/auth-model';

@Injectable({ providedIn: 'root' })
export class AuthApi {
  constructor(private readonly http: HttpClient) {}

  register(payload: RegisterRequest): Observable<ApiResponse<UserSession>> {
    return this.http.post<ApiResponse<UserSession>>(`${API_BASE_URL}/auth/register`, payload);
  }

  registerAdmin(payload: AdminRegisterRequest): Observable<ApiResponse<UserSession>> {
    return this.http.post<ApiResponse<UserSession>>(`${API_BASE_URL}/auth/register-admin`, payload);
  }

  adminLogin(payload: LoginRequest): Observable<ApiResponse<UserSession>> {
    return this.http.post<ApiResponse<UserSession>>(`${API_BASE_URL}/auth/admin-login`, payload);
  }

  login(payload: LoginRequest): Observable<ApiResponse<UserSession>> {
    return this.http.post<ApiResponse<UserSession>>(`${API_BASE_URL}/auth/login`, payload);
  }
}
