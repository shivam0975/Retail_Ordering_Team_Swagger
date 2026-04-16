import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AlertPopup } from './shared/alert-popup/alert-popup';
import { SessionState } from './state/session-state';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, AlertPopup],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(
    public readonly sessionState: SessionState,
    private readonly router: Router
  ) {}

  logout(): void {
    this.sessionState.clear();
    this.router.navigateByUrl('/login');
  }
}
