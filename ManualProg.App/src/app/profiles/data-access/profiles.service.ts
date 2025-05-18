import { IProfileCoinTransactionResponse } from './responses/profile-coin-transaction-response';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IProfileResponse } from './responses/profile-response';
import { IPagedResponse } from '../../shared/models/paged-response';
import { IPagedRequest } from '../../shared/models/paged-request';
import { IUpdateProfileRequest } from './requests/update-profile-request';

@Injectable({
  providedIn: 'root'
})
export class ProfilesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = "/api/profiles";

  getById(id: string): Observable<IProfileResponse> {
    const url = `${this.apiUrl}/${id}`;

    return this.http.get<IProfileResponse>(url);
  }

  getTransactions(id: string, data: IPagedRequest): Observable<IPagedResponse<IProfileCoinTransactionResponse>> {
    let params: any = { };

    if (data.page) params.page = data.page;
    if (data.pageSize) params.pageSize = data.pageSize;

    const url = `${this.apiUrl}/${id}/transactions`;

    return this.http.get<IPagedResponse<IProfileCoinTransactionResponse>>(url, { params: params });
  }

  getImage(id: string): Observable<Blob> {
    const url = `${this.apiUrl}/${id}/image`;

    return this.http.get(url, { responseType: 'blob'});
  }

  update(id: string, data: IUpdateProfileRequest): Observable<any> {
    const url = `${this.apiUrl}/${id}`;

    return this.http.put<any>(url, data);
  }
}
