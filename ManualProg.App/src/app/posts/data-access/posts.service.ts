import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IPagedResponse } from '../../shared/models/paged-response';
import { IPostResponse } from './responses/post-response';
import { ICreatePostRequest } from './requests/create-post-request';
import { IUpdatePostRequest } from './requests/update-post-request';
import { ICommentResponse } from '../../comments/data-access/responses/comment-response';
import { Utils } from '../../utils';
import { IPagedRequest } from '../../shared/models/paged-request';

@Injectable({
  providedIn: 'root'
})
export class PostsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = "/api/posts";

  create(data: ICreatePostRequest): Observable<string> {
    const formData = Utils.objectToFormData(data);

    return this.http.post<string>(this.apiUrl, formData);
  }

  getById(id: string): Observable<IPostResponse> {
    const url = `${this.apiUrl}/${id}`;

    return this.http.get<IPostResponse>(url);
  }

  getComments(id: string, data: IPagedRequest): Observable<IPagedResponse<ICommentResponse>> {
    let params: any = { };

    if (data.page) params.page = data.page;
    if (data.pageSize) params.pageSize = data.pageSize;

    const url = `${this.apiUrl}/${id}/comments`;

    return this.http.get<IPagedResponse<ICommentResponse>>(url, { params: params });
  }

  getImage(imageId: string): Observable<Blob> {
    const url = `${this.apiUrl}/images/${imageId}`;

    return this.http.get(url, { responseType: 'blob'});
  }

  get(data: IPagedRequest): Observable<IPagedResponse<IPostResponse>> {
    let params: any = { };

    if (data.page) params.page = data.page;
    if (data.pageSize) params.pageSize = data.pageSize;

    return this.http.get<IPagedResponse<IPostResponse>>(this.apiUrl, { params: params });
  }

  like(id: string): Observable<any> {
    const url = `${this.apiUrl}/${id}/like`;

    return this.http.post<any>(url, null);
  }

  purchase(id: string): Observable<any> {
    const url = `${this.apiUrl}/${id}/purchase`;

    return this.http.post<any>(url, null);
  }

  unlike(id: string): Observable<any> {
    const url = `${this.apiUrl}/${id}/unlike`;

    return this.http.post<any>(url, null);
  }

  update(id: string, data: IUpdatePostRequest): Observable<any> {
    const url = `${this.apiUrl}/${id}`;

    return this.http.put<any>(url, data);
  }
}
