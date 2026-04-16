import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlertState } from '../../state/alert-state';

@Component({
  selector: 'app-alert-popup',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert-popup.html'
})
export class AlertPopup {
  constructor(public readonly alertState: AlertState) {}

  close(): void {
    this.alertState.close();
  }
}
