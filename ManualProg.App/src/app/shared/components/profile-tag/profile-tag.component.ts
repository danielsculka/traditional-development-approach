import { Component, inject, Input } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { map, Observable, tap } from 'rxjs';
import { ProfilesService } from '../../../profiles/data-access/profiles.service';
import { Router } from '@angular/router';

export interface IProfile {
  id: string;
  name: string;
  hasImage: boolean;
}

@Component({
  selector: 'app-profile-tag',
  standalone: false,
  templateUrl: './profile-tag.component.html',
  styleUrl: './profile-tag.component.scss'
})
export class ProfileTagComponent {
  private readonly _profilesService = inject(ProfilesService);
  private readonly _router = inject(Router);

  @Input() isMinimalistic: boolean = false;

  @Input() set profile(value: IProfile) {
    this._profile = value;

    if (value.hasImage) {
      this.loadProfileImage()
        .subscribe(() => {})
    }
  }

  profileImage: SafeUrl | null = null;

  get profile(): IProfile {
    return this._profile;
  }

  private _profile!: IProfile;

  openProfile() {
    this._router.navigate(['profiles', this.profile.id]);
  }

  private loadProfileImage(): Observable<any> {
    return this._profilesService.getImage(this.profile!.id)
      .pipe(
        tap(response => this.profileImage = URL.createObjectURL(response)),
        map(response => undefined)
      )
  }
}
