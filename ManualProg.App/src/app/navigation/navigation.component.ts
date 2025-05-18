import { AuthService } from './../auth/data-access/auth.service';
import { Component, inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { NavigationItem } from './models/navigation-item';
import { UserRole } from '../shared/enums/user-role';
import { IUser } from '../shared/models/user';

@Component({
  selector: 'app-navigation',
  standalone: false,
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
  visible = false;

  private readonly _authService = inject(AuthService);

  private readonly items: NavigationItem[] = [
    new NavigationItem('Home', 'home', '/posts'),

    new NavigationItem('Users', 'groups', '/users', undefined,
      () => this.currentUser !== null && (this.currentUser.role === UserRole.Administrator || this.currentUser.role === UserRole.Moderator)),

    new NavigationItem('Create', 'add_circle', '/posts/create', undefined,
      () => this.currentUser !== null && this.currentUser.role === UserRole.Basic
    ),

    new NavigationItem('Profile', 'account_circle', undefined,
      () => `/profiles/${this.currentUser?.profileId}`, () => !!this.currentUser?.profileId),
  ];

  get visibleItems(): NavigationItem[] {
    return this.items.filter(item => !item.visible || item.visible())
  }

  get isAuth(): boolean {
    return this.currentUser !== null;
  }

  private get currentUser(): IUser | null {
    return this._authService.currentUserSignal();
  }

  constructor(router: Router) {
    router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.visible = !event.url.includes('auth');
      }
    });
  }

  logout(): void {
    this._authService.logout().subscribe(() => {
      window.location.href = '/';
    });
  }
}
