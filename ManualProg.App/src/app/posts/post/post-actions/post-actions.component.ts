import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { PostsService } from '../../data-access/posts.service';
import { IPostResponse } from '../../data-access/responses/post-response';

@Component({
  selector: 'app-post-actions',
  standalone: false,
  templateUrl: './post-actions.component.html',
  styleUrl: './post-actions.component.scss'
})
export class PostActionsComponent {
  private readonly _postsService = inject(PostsService);

  @Input() post!: IPostResponse;

  @Output() onComment: EventEmitter<any> = new EventEmitter<any>();
  @Output() likeChanged: EventEmitter<any> = new EventEmitter<any>();

  like(): void {
    if (!this.post.hasAccess)
      return;

    this._postsService.like(this.post.id)
      .subscribe(() => this.likeChanged.emit());
  }

  unlike(): void {
    if (!this.post.hasAccess)
      return;

    this._postsService.unlike(this.post.id)
      .subscribe(() => this.likeChanged.emit());
  }
}
