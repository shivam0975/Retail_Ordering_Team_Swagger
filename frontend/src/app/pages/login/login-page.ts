import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthApi } from '../../services/auth-api';
import { AlertState } from '../../state/alert-state';
import { SessionState } from '../../state/session-state';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login-page.html'
})
export class LoginPage {
  readonly mode = signal<'login' | 'register'>('login');

  form = {
    email: '',
    password: ''
  };

  registerForm = {
    name: '',
    email: '',
    password: '',
    confirmPassword: ''
  };

  readonly loading = signal(false);

  constructor(
    private readonly authApi: AuthApi,
    private readonly sessionState: SessionState,
    private readonly alertState: AlertState,
    private readonly router: Router
  ) {}

  setMode(mode: 'login' | 'register'): void {
    this.mode.set(mode);
  }

  register(): void {
    if (!this.registerForm.name.trim() || !this.registerForm.email.trim() || !this.registerForm.password.trim()) {
      this.alertState.error('Name, email, and password are required.');
      return;
    }

    if (this.registerForm.password !== this.registerForm.confirmPassword) {
      this.alertState.error('Passwords do not match.');
      return;
    }

    this.loading.set(true);

    this.authApi.register({
      name: this.registerForm.name.trim(),
      email: this.registerForm.email.trim(),
      password: this.registerForm.password
    }).subscribe({
      next: (response) => {
        this.loading.set(false);

        if (!response.success) {
          this.alertState.error(response.message || 'Registration failed.');
          return;
        }

        this.sessionState.setUser(response.data);
        this.alertState.success(response.message || 'Registration successful.');
        this.router.navigateByUrl('/menu');
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Registration failed. Please try again.');
      }
    });
  }

  login(): void {
    if (!this.form.email.trim() || !this.form.password.trim()) {
      this.alertState.error('Email and password are required.');
      return;
    }

    this.loading.set(true);

    this.authApi.login(this.form).subscribe({
      next: (response) => {
        this.loading.set(false);

        if (!response.success) {
          this.alertState.error(response.message || 'Login failed.');
          return;
        }

        this.sessionState.setUser(response.data);
        this.alertState.success(response.message || 'Login successful.');
        this.router.navigateByUrl('/menu');
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Login failed. Please try again.');
      }
    });
  }
}
