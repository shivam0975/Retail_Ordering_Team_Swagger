import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { DashboardSummary } from '../../models/dashboard-model';
import { DashboardApi } from '../../services/dashboard-api';
import { AlertState } from '../../state/alert-state';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard-page.html'
})
export class DashboardPage implements OnInit {
  readonly summary = signal<DashboardSummary | null>(null);
  readonly loading = signal(false);

  constructor(
    private readonly dashboardApi: DashboardApi,
    private readonly alertState: AlertState
  ) {}

  ngOnInit(): void {
    this.loadSummary();
  }

  private loadSummary(): void {
    this.loading.set(true);

    this.dashboardApi.getSummary().subscribe({
      next: (response) => {
        this.loading.set(false);

        if (!response.success) {
          this.alertState.error(response.message || 'Failed to load dashboard summary.');
          return;
        }

        this.summary.set(response.data);
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.alertState.error(error.error?.message || 'Failed to load dashboard summary.');
      }
    });
  }
}
