import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { SessionState } from './session-state';

export const authGuard: CanActivateFn = () => {
  const session = inject(SessionState);
  const router = inject(Router);

  if (session.user()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};

export const guestGuard: CanActivateFn = () => {
  const session = inject(SessionState);
  const router = inject(Router);

  if (!session.user()) {
    return true;
  }

  return router.createUrlTree(['/menu']);
};

export const adminGuard: CanActivateFn = () => {
  const session = inject(SessionState);
  const router = inject(Router);
  const user = session.user();

  if (!user) {
    return router.createUrlTree(['/admin/login']);
  }

  if (user.role !== 'ADMIN') {
    return router.createUrlTree(['/menu']);
  }

  return true;
};

export const adminGuestGuard: CanActivateFn = () => {
  const session = inject(SessionState);
  const router = inject(Router);
  const user = session.user();

  if (user?.role === 'ADMIN') {
    return router.createUrlTree(['/admin/dashboard']);
  }

  return true;
};
