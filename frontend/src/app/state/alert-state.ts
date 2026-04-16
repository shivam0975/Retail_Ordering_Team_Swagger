import { Injectable, signal } from '@angular/core';

export type AlertType = 'success' | 'error';

export interface AlertStateModel {
  visible: boolean;
  type: AlertType;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class AlertState {
  readonly alert = signal<AlertStateModel>({
    visible: false,
    type: 'success',
    message: ''
  });

  success(message: string): void {
    this.show('success', message);
  }

  error(message: string): void {
    this.show('error', message);
  }

  close(): void {
    this.alert.set({ visible: false, type: 'success', message: '' });
  }

  private show(type: AlertType, message: string): void {
    this.alert.set({ visible: true, type, message });
  }
}
