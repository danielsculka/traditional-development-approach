import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { concatMap, forkJoin, map, Observable, of, tap } from 'rxjs';
import { ProfilesService } from '../data-access/profiles.service';
import { IProfileResponse } from '../data-access/responses/profile-response';
import { SafeUrl } from '@angular/platform-browser';
import { IPostResponse } from '../../posts/data-access/responses/post-response';
import { PostsService } from '../../posts/data-access/posts.service';
import { IPostsRequest } from '../../posts/data-access/requests/posts-request';
import { AuthService } from '../../auth/data-access/auth.service';
import { UserRole } from '../../shared/enums/user-role';
import { IPagedRequest } from '../../shared/models/paged-request';
import { IProfileCoinTransactionResponse } from '../data-access/responses/profile-coin-transaction-response';
import { MatTabChangeEvent } from '@angular/material/tabs';

@Component({
  selector: 'app-profile',
  standalone: false,
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private readonly _authService = inject(AuthService);
  private readonly _postsService = inject(PostsService);
  private readonly _profileService = inject(ProfilesService);
  private readonly _route = inject(ActivatedRoute);

  @ViewChild('scrollBox') scrollBox!: ElementRef;

  profile: IProfileResponse = <IProfileResponse>{};
  profileImage: SafeUrl | null = null;
  posts: IPostResponse[] = [];
  transactions: IProfileCoinTransactionResponse[] = [];

  get canAccessTransactions(): boolean {
    const user = this._authService.currentUserSignal();

    return user?.role == UserRole.Administrator
      || user?.profileId === this.profileId;
  }

  readonly transactionsDisplayedColumns: string[] = ['sender', 'receiver', 'ammount', 'created'];

  private profileId!: string;

  private isPostsTabActive: boolean = true;

  private postsPage: number = 0;
  private postsPageSize: number = 5;
  private postsHasNextPage: boolean = true;

  private transactionsPage: number = 0;
  private transactionsPageSize: number = 5;
  private transactionsHasNextPage: boolean = true;

  ngOnInit(): void {
    this._route.paramMap.subscribe(paramMap => {
      const profileId = paramMap.get('id') as string;

      this.profileId = profileId;

      this.init();
    });
  }

  onScroll(): void {
    const element = this.scrollBox.nativeElement;
    const isAtBottom = element.scrollTop + element.clientHeight >= element.scrollHeight;

    if (isAtBottom) {
      if (this.isPostsTabActive) {
        this.fetchPosts().subscribe(() => {});
      } else {
        this.fetchCoinTransactions().subscribe(() => {});
      }
    }
  }

  onTabChange(event: MatTabChangeEvent) {
    this.isPostsTabActive = event.index === 0;

    this.onScroll();
  }

  init(): void {
    this.isPostsTabActive = true;

    this.posts = [];
    this.postsPage = 0;
    this.postsHasNextPage = true;

    this.transactions = [];
    this.transactionsPage = 0;
    this.transactionsHasNextPage = true;

    forkJoin({
      profile: this.loadProfile(),
      posts: this.fetchPosts()
    }).pipe(
      concatMap(() => this.profile.hasImage ? this.loadImage() : of())
    ).subscribe(() => {});
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

    const request = <IPostsRequest>{
      page: this.postsPage,
      pageSize: this.postsPageSize,
      profileId: this.profileId
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

  private fetchCoinTransactions(): Observable<any> {
    if (!this.transactionsHasNextPage)
      return of();

    this.transactionsPage++;

    const request = <IPagedRequest>{
      page: this.transactionsPage,
      pageSize: this.transactionsPageSize
    };

    return this._profileService.getTransactions(this.profileId, request)
      .pipe(
        tap(response => {
          this.transactions = this.transactions.concat(response.items);
          this.transactionsHasNextPage = response.hasNextPage;
        }),
        map(response => undefined)
      )
  }
}
