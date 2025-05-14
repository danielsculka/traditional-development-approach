import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { concatMap, forkJoin, map, Observable, of, tap } from 'rxjs';
import { ProfilesService } from '../data-access/profiles.service';
import { IProfileResponse } from '../data-access/responses/profile-response';
import { IProfilePostResponse } from '../data-access/responses/profile-post-response';
import { IPagedRequest } from '../../shared/models/paged-request';
import { SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-profile',
  standalone: false,
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private readonly _profileService = inject(ProfilesService);
  private readonly _route = inject(ActivatedRoute);

  readonly profileId: string = this._route.snapshot.paramMap.get('id') as string;

  profile: IProfileResponse = <IProfileResponse>{};
  profileImage: SafeUrl | null = null;
  posts: IProfilePostResponse[] = [];

  private postsPage: number = 0;
  private postsPageSize: number = 5;
  private postsHasNextPage: boolean = true;

  ngOnInit(): void {
    forkJoin({
      profile: this.loadProfile(),
      posts: this.fetchPosts()
    }).pipe(
      concatMap(() => this.profile.hasImage ? this.loadImage() : of())
    ).subscribe(() => {});
  }

  onScroll(element: HTMLElement): void {
    const isAtBottom = element.scrollTop + element.clientHeight >= element.scrollHeight;

    if (isAtBottom) {
      this.fetchPosts().subscribe(() => {});
    }
  }

  private loadProfile(): Observable<any> {
    return this._profileService.getById(this.profileId)
      .pipe(
        tap(response => this.profile = response),
        map(response => undefined)
      )
  }

  private loadImage(): Observable<any> {
    if (!this.profile.hasImage)
      return of();

    return this._profileService.getImage(this.profileId)
      .pipe(
        tap(response => this.profileImage = URL.createObjectURL(response)),
        map(response => undefined)
      )
  }

  private fetchPosts(): Observable<any> {
    if (!this.postsHasNextPage)
      return of();

    this.postsPage++;

    const request = <IPagedRequest>{
      page: this.postsPage,
      pageSize: this.postsPageSize
    };

    return this._profileService.getPosts(this.profileId, request)
      .pipe(
        tap(response => {
          this.posts = this.posts.concat(response.items);
          this.postsHasNextPage = response.hasNextPage;
        }),
        map(response => undefined)
      )
  }
}
