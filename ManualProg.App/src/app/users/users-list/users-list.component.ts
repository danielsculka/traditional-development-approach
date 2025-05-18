import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { UsersService } from '../data-access/users.service';
import { map, Observable, of, tap } from 'rxjs';
import { IUserResponse } from '../data-access/responses/user-response';
import { IPagedRequest } from '../../shared/models/paged-request';
import { AuthService } from '../../auth/data-access/auth.service';
import { UserRole } from '../../shared/enums/user-role';
import { MatDialog } from '@angular/material/dialog';
import { UserEditModalComponent } from '../user-edit-modal/user-edit-modal.component';

interface IUserItem extends IUserResponse {
  canEdit: boolean;
  canDelete: boolean;
}

@Component({
  selector: 'app-users-list',
  standalone: false,
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.scss'
})
export class UsersListComponent implements OnInit {
  private readonly _authService = inject(AuthService);
  private readonly _usersService = inject(UsersService);
  private readonly _dialog = inject(MatDialog);

  @ViewChild('scrollBox') scrollBox!: ElementRef;

  items: IUserItem[] = [];
  page: number = 0;
  pageSize: number = 15;
  hasNextPage: boolean = true;

  readonly displayedColumns: string[] = ['username', 'role', 'profile', 'created', 'actions'];

  get canCreate(): boolean {
    const user = this._authService.currentUserSignal();

    return user?.role === UserRole.Administrator;
  }

  ngOnInit(): void {
    this.fetchItems().subscribe();
  }

  onScroll(): void {
    const element = this.scrollBox.nativeElement;
    const isAtBottom = element.scrollTop + element.clientHeight >= element.scrollHeight;

    if (isAtBottom) {
      this.fetchItems().subscribe(() => {});
    }
  }

  openModal(item: IUserResponse | null = null): void {
    const dialogRef = this._dialog.open(UserEditModalComponent, {
      data: item
    });

    dialogRef.afterClosed().subscribe(response => {
      if (response) {
        this.refresh();
      }
    })
  }

  delete(item: IUserResponse): void {
    this._usersService.delete(item.id)
      .subscribe(() => this.refresh());
  }

  private refresh(): void {
    this.items = [];
    this.page = 0;
    this.hasNextPage = true;

    this.fetchItems().subscribe(() => {});
  }

  private fetchItems(): Observable<any> {
    if (!this.hasNextPage)
      return of();

    this.page++;

    const request: IPagedRequest = {
      page: this.page,
      pageSize: this.pageSize
    };

    return this._usersService.get(request)
      .pipe(
        tap(response => {
          const user = this._authService.currentUserSignal();

          const items = response.items.map(i => <IUserItem>{
            ...i,
            canEdit: i.role !== UserRole.Basic && i.username !== 'admin',
            canDelete: i.role !== UserRole.Administrator
              && user?.role === UserRole.Administrator
          });

          this.items = this.items.concat(items);
          this.hasNextPage = response.hasNextPage;
        }),
        map(reponse => undefined)
      );
  }
}
