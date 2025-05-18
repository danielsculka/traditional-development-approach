import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../auth/data-access/auth.service';
import { UserRole } from '../shared/enums/user-role';

@Injectable({
  providedIn: 'root'
})
export class UserRoleGuard implements CanActivate {
  private readonly _authService = inject(AuthService);
  private readonly _router = inject(Router);

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const roles = route.data['roles'] as UserRole[];

    const user = this._authService.currentUserSignal();

    if (user !== null && roles.includes(user.role))
      return true;

    this._router.navigate(['/']);

    return false;
  }
}
