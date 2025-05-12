import { Component, inject, OnInit } from '@angular/core';
import { UsersService } from '../data-access/users.service';
import { map, Observable, tap } from 'rxjs';
import { IUserResponse } from '../data-access/responses/user-response';
import { IPagedRequest } from '../../shared/models/paged-request';

@Component({
  selector: 'app-users-list',
  standalone: false,
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.scss'
})
export class UsersListComponent implements OnInit {
  private readonly _usersService = inject(UsersService);

  items: IUserResponse[] = [];
  page: number = 0;
  pageSize: number = 15;
  totalItems: number = 0;

  readonly displayedColumns: string[] = ['username', 'role', 'profile', 'created'];

  ngOnInit(): void {
    this.load().subscribe();
  }

  // onPage(): void {
  //   this.page =

  //   this.load().subscribe();
  // }

  load(): Observable<any> {
    const request: IPagedRequest = {
      page: this.page,
      pageSize: this.pageSize
    };

    return this._usersService.get(request)
      .pipe(
        tap(response => {
          this.items = response.items;
          this.page = response.page;
          this.totalItems = response.totalItems;
        }),
        map(reponse => undefined)
      );
  }
}
