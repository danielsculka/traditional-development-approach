import { Component, inject, Input, OnInit } from '@angular/core';
import { PostsService } from '../data-access/posts.service';
import { IPostResponse } from '../data-access/responses/post-response';
import { IProfilePostResponse } from '../../profiles/data-access/responses/profile-post-response';
import { map, Observable, tap } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { PostModalComponent } from './post-modal/post-modal.component';

export type IPost = IPostResponse | IProfilePostResponse;

@Component({
  selector: 'app-post',
  standalone: false,
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss'
})
export class PostComponent implements OnInit {
  private readonly _postsService = inject(PostsService);
  private readonly _dialog = inject(MatDialog);

  @Input() post!: IPost;

  isIndependent: boolean = true;

  ngOnInit(): void {
    this.isIndependent = this.post.hasOwnProperty('profile');
  }

  onLikeChange(): void {
    this.refreshPost()
      .subscribe(() => {});
  }

  openModal(): void {
    console.log(this.post);

    const dialogRef = this._dialog.open(PostModalComponent, {
      data: this.post
    });

    dialogRef.afterClosed().subscribe(() => {
      this.refreshPost()
        .subscribe(() => {});
    })
  }

  private refreshPost(): Observable<any> {
    return this._postsService.getById(this.post.id)
      .pipe(
        tap(response => this.post = response),
        map(response => undefined)
      )
  }
}
