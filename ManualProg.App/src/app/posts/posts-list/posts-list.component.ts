import { Component, inject } from '@angular/core';
import { PostsService } from '../data-access/posts.service';
import { map, Observable, of, tap } from 'rxjs';
import { IPagedRequest } from '../../shared/models/paged-request';
import { IPostResponse } from '../data-access/responses/post-response';

@Component({
  selector: 'app-posts-list',
  standalone: false,
  templateUrl: './posts-list.component.html',
  styleUrl: './posts-list.component.scss'
})
export class PostsListComponent {
  private readonly _postsService = inject(PostsService);

  posts: IPostResponse[] = [];

  private postsPage: number = 0;
  private postsPageSize: number = 2;
  private postsHasNextPage: boolean = true;

  ngOnInit(): void {
    this.load();
  }

  onScroll(element: HTMLElement): void {
    const isAtBottom = element.scrollTop + element.clientHeight >= element.scrollHeight;

    if (isAtBottom) {
      this.load();
    }
  }

  private load(): void {
    this.fetchPosts().subscribe(() => {});
  }

  private fetchPosts(): Observable<any> {
    if (!this.postsHasNextPage)
      return of();

    this.postsPage++;

    const request = <IPagedRequest>{
      page: this.postsPage,
      pageSize: this.postsPageSize
    };

    return this._postsService.get(request)
      .pipe(
        tap(response => {
          this.posts = this.posts.concat(response.items);
          this.postsHasNextPage = response.hasNextPage;
        }),
        map(response => undefined)
      )
  }
}
