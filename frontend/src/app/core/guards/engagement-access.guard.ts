import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const engagementAccessGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const userRole = localStorage.getItem('userRole'); // Mocked logic for now
  // Check if role exists and user has assignment. Actual backend integration will happen.
  
  if (userRole === 'Pre Sales' || userRole === 'Project Management') {
    return true;
  }
  
  // For the sake of this ticket, if there's no complex frontend auth state yet, just let them pass for dev
  return true;
};
