import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ICommentResponse } from '../data-access/responses/comment-response';
import { map, Observable, of, tap } from 'rxjs';
import { CommentsService } from '../data-access/comments.service';
import { IPagedRequest } from '../../shared/models/paged-request';
import { AuthService } from '../../auth/data-access/auth.service';
import { UserRole } from '../../shared/enums/user-role';

export type IComment = ICommentResponse;

@Component({
  selector: 'app-comment',
  standalone: false,
  templateUrl: './comment.component.html',
  styleUrl: './comment.component.scss'
})
export class CommentComponent {
  private readonly _authService = inject(AuthService);
  private readonly _commentsService = inject(CommentsService);

  @Input() comment!: IComment;

  @Output() onReply: EventEmitter<IComment> = new EventEmitter<IComment>();

  replies: ICommentResponse[] = [];

  repliesOpen: boolean = false;

  get canReply(): boolean {
    const user = this._authService.currentUserSignal();

    return !!user && user.role === UserRole.Basic;
  }

  get canLike(): boolean {
    const user = this._authService.currentUserSignal();

    return !!user && user.role === UserRole.Basic;
  }


  get canDelete(): boolean {
    const user = this._authService.currentUserSignal();

    return user?.role === UserRole.Administrator
      || user?.role === UserRole.Moderator
      || this._authService.currentUserSignal()?.profileId === this.comment.profile.id
  }

  private repliesPage: number = 0;
  private repliesPageSize: number = 5;
  private repliesHasNextPage: boolean = true;

  toggleReplies(): void {
    this.repliesOpen = !this.repliesOpen;

    if (!this.repliesOpen)
      return;

    this.fetchReplies()
      .subscribe(() => {});
  }

  like(): void {
    this._commentsService.like(this.comment.id)
      .subscribe(() => {});
  }

  unlike(): void {
    this._commentsService.unlike(this.comment.id)
      .subscribe(() => {});
  }

  delete(): void {
    this._commentsService.delete(this.comment.id)
      .subscribe(() => {});
  }

  private fetchReplies(): Observable<any> {
    if (!this.repliesHasNextPage)
      return of();

    this.repliesPage++;

    const request = <IPagedRequest>{
      page: this.repliesPage,
      pageSize: this.repliesPageSize
    };

    return this._commentsService.getReplies(this.comment.id, request)
      .pipe(
        tap(response => {
          this.replies = this.replies.concat(response.items);
          this.repliesHasNextPage = response.hasNextPage;
        }),
        map(response => undefined)
      )
  }
}
