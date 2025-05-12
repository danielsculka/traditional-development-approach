import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ICreateCommentRequest } from './requests/create-comment-request';
import { ICommentsRequest } from './requests/comments-request';
import { IPagedResponse } from '../../shared/models/paged-response';
import { ICommentResponse } from './responses/comment-response';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = "/api/comments";

  create(data: ICreateCommentRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, data);
  }

  getReplies(id: string, data: ICommentsRequest): Observable<IPagedResponse<ICommentResponse>> {
    let params: any = { };

    if (data.page) params.page = data.page;
    if (data.pageSize) params.pageSize = data.pageSize;

    const url = `${this.apiUrl}/${id}/replies`;

    return this.http.get<IPagedResponse<ICommentResponse>>(url, { params: params });
  }

  like(id: string): Observable<any> {
    const url = `${this.apiUrl}/${id}/like`;

    return this.http.post<any>(url, null);
  }

  unlike(id: string): Observable<any> {
    const url = `${this.apiUrl}/${id}/unlike`;

    return this.http.post<any>(url, null);
  }
}
