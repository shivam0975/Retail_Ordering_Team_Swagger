import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AdminDashboard } from '../../../models/admin-model';
import { AdminApi } from '../../../services/admin-api';
import { AlertState } from '../../../state/alert-state';

@Component({
  selector: 'app-admin-dashboard-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-dashboard-page.html'
})
export class AdminDashboardPage implements OnInit {
  readonly summary = signal<AdminDashboard | null>(null);
  readonly loading = signal(false);

  constructor(
    private readonly adminApi: AdminApi,
    private readonly alertState: AlertState
  ) {}

  ngOnInit(): void {
    this.loadSummary();
  }

  private loadSummary(): void {
    this.loading.set(true);

    this.adminApi.getDashboard().subscribe({
      next: (response) => {
        this.loading.set(false);
        if (!response.success) {
          this.alertState.error(response.message || 'Failed to load admin dashboard.');
          return;
        }

        this.summary.set(response.data);
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Failed to load admin dashboard.');
      }
    });
  }
}
