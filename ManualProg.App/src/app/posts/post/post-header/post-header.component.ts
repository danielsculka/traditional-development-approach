import { Component, inject, Input, OnInit } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { IPostResponse, IPostResponseProfileData } from '../../data-access/responses/post-response';
import { IPost } from '../post.component';
import { map, Observable, of, tap } from 'rxjs';
import { ProfilesService } from '../../../profiles/data-access/profiles.service';

@Component({
  selector: 'app-post-header',
  standalone: false,
  templateUrl: './post-header.component.html',
  styleUrl: './post-header.component.scss'
})
export class PostHeaderComponent implements OnInit {
  private readonly _profilesService = inject(ProfilesService);

  profileImage: SafeUrl | null = null;
  profile!: IPostResponseProfileData;

  @Input() post!: IPost;

  ngOnInit(): void {
    const hasProfile = this.post.hasOwnProperty('profile');

    if (hasProfile) {
      const p = this.post as IPostResponse;

      this.profile = p.profile;

      this.loadProfileImage()
        .subscribe(() => {})
    }
  }

  private loadProfileImage(): Observable<any> {
    if (!this.profile!.hasImage)
      return of();

    return this._profilesService.getImage(this.profile!.id)
      .pipe(
        tap(response => this.profileImage = URL.createObjectURL(response)),
        map(response => undefined)
      )
  }
}
