import { Injectable, signal } from '@angular/core';
import { UserSession } from '../models/auth-model';

const SESSION_KEY = 'retail-order-session';

@Injectable({ providedIn: 'root' })
export class SessionState {
  readonly user = signal<UserSession | null>(this.loadSession());

  setUser(user: UserSession): void {
    this.user.set(user);
    localStorage.setItem(SESSION_KEY, JSON.stringify(user));
  }

  clear(): void {
    this.user.set(null);
    localStorage.removeItem(SESSION_KEY);
  }

  private loadSession(): UserSession | null {
    const raw = localStorage.getItem(SESSION_KEY);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as UserSession;
    } catch {
      localStorage.removeItem(SESSION_KEY);
      return null;
    }
  }
}
