import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ICreateUserRequest } from './requests/create-user-request';
import { Observable } from 'rxjs';
import { IUpdateUserRequest } from './requests/update-user-request';
import { IUserResponse } from './responses/user-response';
import { IPagedResponse } from '../../shared/models/paged-response';
import { IPagedRequest } from '../../shared/models/paged-request';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = "/api/users";

  get(data: IPagedRequest): Observable<IPagedResponse<IUserResponse>> {
    let params: any = { };

    if (data.page) params.page = data.page;
    if (data.pageSize) params.pageSize = data.pageSize;

    return this.http.get<IPagedResponse<IUserResponse>>(this.apiUrl, { params: params });
  }

  create(data: ICreateUserRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, data);
  }

  update(id: string, data: IUpdateUserRequest): Observable<any> {
    const url = `${this.apiUrl}/${id}`;

    return this.http.put<any>(url, data);
  }

  delete(id: string): Observable<any> {
    const url = `${this.apiUrl}/${id}`;

    return this.http.delete<any>(url);
  }
}
