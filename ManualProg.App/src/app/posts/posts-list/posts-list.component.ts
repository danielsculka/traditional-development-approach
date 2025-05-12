import { Component, inject } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { PostsService } from '../data-access/posts.service';
import { concatMap, forkJoin, map, Observable, of, tap } from 'rxjs';
import { IPagedRequest } from '../../shared/models/paged-request';
import { IPostResponse } from '../data-access/responses/post-response';
import { ProfilesService } from '../../profiles/data-access/profiles.service';

@Component({
  selector: 'app-posts-list',
  standalone: false,
  templateUrl: './posts-list.component.html',
  styleUrl: './posts-list.component.scss'
})
export class PostsListComponent {
  private readonly _profilesService = inject(ProfilesService);
  private readonly _postsService = inject(PostsService);

  profileImages: { [id: string] : SafeUrl } = {};
  posts: IPostResponse[] = [];
  postsImages: { [id: string] : SafeUrl } = {};

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
    this.fetchPosts()
      .pipe(
        concatMap(() => {
          const postImages = this.posts.filter(p => !this.postsImages[p.id])
            .map(p => p.imageIds.map(id => this.fetchPostImage(id)))
            .reduce((acc, curr) => acc.concat(curr), []);

          const profileImages = this.posts.filter(p => p.profile.hasImage && !this.profileImages[p.profile.id])
            .map(p => this.fetchProfileImage(p.profile.id));

          return forkJoin([...postImages, ...profileImages]);
        })
      ).subscribe(() => {});
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

  private fetchPostImage(imageId: string): Observable<any> {
    return this._postsService.getImage(imageId)
      .pipe(
        tap(response => this.postsImages[imageId] = URL.createObjectURL(response)),
        map(response => undefined)
      )
  }

  private fetchProfileImage(profileId: string): Observable<any> {
    return this._profilesService.getImage(profileId)
      .pipe(
        tap(response => this.profileImages[profileId] = URL.createObjectURL(response)),
        map(response => undefined)
      )
  }
}
