import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthApi } from '../../../services/auth-api';
import { AlertState } from '../../../state/alert-state';
import { SessionState } from '../../../state/session-state';

@Component({
  selector: 'app-admin-login-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-login-page.html'
})
export class AdminLoginPage {
  readonly mode = signal<'login' | 'register'>('login');
  readonly loading = signal(false);

  loginForm = {
    email: '',
    password: ''
  };

  registerForm = {
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
    adminSecret: ''
  };

  constructor(
    private readonly authApi: AuthApi,
    private readonly sessionState: SessionState,
    private readonly alertState: AlertState,
    private readonly router: Router
  ) {}

  setMode(mode: 'login' | 'register'): void {
    this.mode.set(mode);
  }

  adminLogin(): void {
    if (!this.loginForm.email.trim() || !this.loginForm.password.trim()) {
      this.alertState.error('Email and password are required.');
      return;
    }

    this.loading.set(true);

    this.authApi.adminLogin(this.loginForm).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (!response.success) {
          this.alertState.error(response.message || 'Admin login failed.');
          return;
        }

        this.sessionState.setUser(response.data);
        this.alertState.success(response.message || 'Admin login successful.');
        this.router.navigateByUrl('/admin/dashboard');
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Admin login failed.');
      }
    });
  }

  adminRegister(): void {
    if (!this.registerForm.name.trim() || !this.registerForm.email.trim() || !this.registerForm.password.trim() || !this.registerForm.adminSecret.trim()) {
      this.alertState.error('Name, email, password, and admin secret are required.');
      return;
    }

    if (this.registerForm.password !== this.registerForm.confirmPassword) {
      this.alertState.error('Passwords do not match.');
      return;
    }

    this.loading.set(true);

    this.authApi.registerAdmin({
      name: this.registerForm.name.trim(),
      email: this.registerForm.email.trim(),
      password: this.registerForm.password,
      adminSecret: this.registerForm.adminSecret.trim()
    }).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (!response.success) {
          this.alertState.error(response.message || 'Admin registration failed.');
          return;
        }

        this.sessionState.setUser(response.data);
        this.alertState.success(response.message || 'Admin registration successful.');
        this.router.navigateByUrl('/admin/dashboard');
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Admin registration failed.');
      }
    });
  }
}
