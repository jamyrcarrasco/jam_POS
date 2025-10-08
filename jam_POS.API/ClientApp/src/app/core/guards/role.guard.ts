import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { APP_CONSTANTS } from '../constants/app-constants.constant';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const requiredRoles = route.data['roles'] as string[];
    
    if (!requiredRoles || requiredRoles.length === 0) {
      return true;
    }

    const user = this.authService.getCurrentUser();
    
    if (!user) {
      this.router.navigate(['/login']);
      return false;
    }

    const hasRequiredRole = requiredRoles.some(role => this.authService.hasRole(role));
    
    if (!hasRequiredRole) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  }
}
