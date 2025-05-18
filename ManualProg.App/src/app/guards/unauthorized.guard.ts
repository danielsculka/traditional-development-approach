import { inject, Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../auth/data-access/auth.service';

@Injectable({
  providedIn: 'root'
})
export class UnauthorizedGuard implements CanActivate {
  private readonly _authService = inject(AuthService);
  private readonly _router = inject(Router);

  canActivate(): boolean {
    const user = this._authService.currentUserSignal();

    if (user === null)
      return true;

    this._router.navigate(['/']);

    return false;
  }
}
