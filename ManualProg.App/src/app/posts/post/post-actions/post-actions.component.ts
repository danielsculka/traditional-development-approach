import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { IUser } from '../../../shared/models/user';
import { AuthService } from '../../../auth/data-access/auth.service';
import { PostsService } from '../../data-access/posts.service';
import { IPost, PostComponent } from '../post.component';
import { IPostResponse } from '../../data-access/responses/post-response';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-post-actions',
  standalone: false,
  templateUrl: './post-actions.component.html',
  styleUrl: './post-actions.component.scss'
})
export class PostActionsComponent {
  private readonly _authService = inject(AuthService);
  private readonly _postsService = inject(PostsService);
  private readonly _dialog = inject(MatDialog);

  @Input() set post(value: IPost) {
    this._post = value;

    if (value.hasOwnProperty('hasLike')) {
      const post = value as IPostResponse;

      this.hasLike = post.hasLike;
    }
  }

  @Output() onComment: EventEmitter<any> = new EventEmitter<any>();
  @Output() likeChanged: EventEmitter<any> = new EventEmitter<any>();

  hasLike: boolean = false;

  get post(): IPost {
    return this._post;
  }

  private _post!: IPost;

  private get currentUser(): IUser | null {
    return this._authService.currentUserSignal();
  }

  like(): void {
    if (!this.currentUser)
      return;

    this._postsService.like(this.post.id)
      .subscribe(() => this.likeChanged.emit());
  }

  unlike(): void {
    if (!this.currentUser)
      return;

    this._postsService.unlike(this.post.id)
      .subscribe(() => this.likeChanged.emit());
  }
}
