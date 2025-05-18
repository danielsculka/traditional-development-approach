import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { IPostResponse } from '../../data-access/responses/post-response';
import { AuthService } from '../../../auth/data-access/auth.service';
import { UserRole } from '../../../shared/enums/user-role';
import { PostsService } from '../../data-access/posts.service';

@Component({
  selector: 'app-post-header',
  standalone: false,
  templateUrl: './post-header.component.html',
  styleUrl: './post-header.component.scss'
})
export class PostHeaderComponent {
  private readonly _authService = inject(AuthService);
  private readonly _postsService = inject(PostsService);

  @Input() post!: IPostResponse;
  @Input() showDelete: boolean = false;
  @Output() onDelete: EventEmitter<any> = new EventEmitter<any>();

  get canDelete(): boolean {
    if (!this.showDelete)
      return false;

    const user = this._authService.currentUserSignal();

    return user?.role === UserRole.Administrator
      || user?.role === UserRole.Moderator
      || this._authService.currentUserSignal()?.profileId === this.post.profile.id
  }

  delete(): void {
    this._postsService.delete(this.post.id)
      .subscribe(() => this.onDelete.emit());
  }
}
