import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { map, Observable, of, tap } from 'rxjs';
import { PostsService } from '../data-access/posts.service';
import { IPagedRequest } from '../../shared/models/paged-request';
import { CommentsService } from '../../comments/data-access/comments.service';
import { IPostResponse } from '../data-access/responses/post-response';
import { ICommentResponse } from '../../comments/data-access/responses/comment-response';
import { FormBuilder, Validators } from '@angular/forms';
import { ICreateCommentRequest } from '../../comments/data-access/requests/create-comment-request';
import { IComment } from '../../comments/comment/comment.component';
import { AuthService } from '../../auth/data-access/auth.service';
import { UserRole } from '../../shared/enums/user-role';

@Component({
  selector: 'app-post-modal',
  standalone: false,
  templateUrl: './post-modal.component.html',
  styleUrl: './post-modal.component.scss'
})
export class PostModalComponent {
  private readonly _fb = inject(FormBuilder);
  private readonly _authService = inject(AuthService);
  private readonly _commentsService = inject(CommentsService);
  private readonly _postsService = inject(PostsService);
  private readonly _dialogRef = inject(MatDialogRef<PostModalComponent>);

  post: IPostResponse = inject(MAT_DIALOG_DATA);
  comments: ICommentResponse[] = [];

  newComment = this._fb.nonNullable.group({
    content: ['', Validators.required],
    replyToComment: this._fb.control<IComment | null>(null)
  });

  commentsHasNextPage: boolean = true;

  get canComment(): boolean {
    const user = this._authService.currentUserSignal();

    return !!user && user.role === UserRole.Basic;
  }

  private commentsPage: number = 0;
  private commentsPageSize: number = 10;

  constructor() {
    this.fetchComments()
      .subscribe(() => {});
  }

  reply(comment: IComment) {
    this.newComment.controls.replyToComment.setValue(comment);

    this.newComment.controls.content.setValue(`@${comment.profile.name} `)
  }

  onLikeChange(): void {
    this.refreshPost()
      .subscribe(() => {});
  }

  onComment(): void {
    const value = this.newComment.value;

    const request: ICreateCommentRequest = {
      postId: this.post.id,
      content: value.content!,
      replyToCommentId: value.replyToComment
        ? value.replyToComment.id
        : null
    }

    this._commentsService.create(request)
      .subscribe(() => {});
  }

  onCommentLoad(): void {
    this.fetchComments()
      .subscribe(() => {});
  }

  onDelete(): void {
    this._dialogRef.close(true);
  }

  private fetchComments(): Observable<any> {
    if (!this.commentsHasNextPage)
      return of();

    this.commentsPage++;

    const request = <IPagedRequest>{
      page: this.commentsPage,
      pageSize: this.commentsPageSize
    };

    return this._postsService.getComments(this.post.id, request)
      .pipe(
        tap(response => {
          this.comments = this.comments.concat(response.items);
          this.commentsHasNextPage = response.hasNextPage;
        }),
        map(response => undefined)
      )
  }

  private refreshPost(): Observable<any> {
    return this._postsService.getById(this.post.id)
      .pipe(
        tap(response => this.post = response),
        map(response => undefined)
      )
  }
}
