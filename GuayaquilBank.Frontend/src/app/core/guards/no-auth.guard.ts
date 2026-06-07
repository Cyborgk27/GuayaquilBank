import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { AuthFacade } from '../service/auth.facade';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuardService {
  private authService = inject(AuthFacade);
  private router = inject(Router);

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    if (!this.authService.isAuthenticated()) {
      return true;
    }

    this.router.navigate(['']);
    return false;
  }
}

export const noAuthGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const service = inject(NoAuthGuardService);
  return service.canActivate(route, state);
};
