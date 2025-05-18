import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { PostsService } from '../data-access/posts.service';
import { IPostResponse } from '../data-access/responses/post-response';
import { map, Observable, switchMap, tap } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { PostModalComponent } from '../post-modal/post-modal.component';
import { AuthService } from '../../auth/data-access/auth.service';

@Component({
  selector: 'app-post',
  standalone: false,
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss'
})
export class PostComponent {
  private readonly _authService = inject(AuthService);
  private readonly _postsService = inject(PostsService);
  private readonly _dialog = inject(MatDialog);

  @Input() post!: IPostResponse;

  @Input() isMinimalistic: boolean = true;

  @Output() onDelete: EventEmitter<any> = new EventEmitter<any>();

  onLikeChange(): void {
    this.refreshPost()
      .subscribe(() => {});
  }

  openModal(): void {
    if (!this.post.hasAccess)
      return;

    const dialogRef = this._dialog.open(PostModalComponent, {
      data: this.post
    });

    dialogRef.afterClosed().subscribe(deleted => {
      if (deleted) {
        this.onDelete.emit();
      } else {
        this.refreshPost()
          .subscribe(() => {});
      }
    })
  }

  onPurchase(): void {
    if (this.post.hasAccess || !this._authService.currentUserSignal())
      return;

    this._postsService.purchase(this.post.id)
      .pipe(
        switchMap(() => this.refreshPost())
      ).subscribe(() => {});
  }

  private refreshPost(): Observable<any> {
    return this._postsService.getById(this.post.id)
      .pipe(
        tap(response => this.post = response),
        map(response => undefined)
      )
  }
}
