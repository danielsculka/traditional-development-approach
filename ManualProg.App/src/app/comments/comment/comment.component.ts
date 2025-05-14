import { Component, inject, Input, OnInit } from '@angular/core';
import { ICommentResponse } from '../data-access/responses/comment-response';
import { ProfilesService } from '../../profiles/data-access/profiles.service';
import { SafeUrl } from '@angular/platform-browser';
import { map, Observable, of, tap } from 'rxjs';

export type IComment = ICommentResponse;

@Component({
  selector: 'app-comment',
  standalone: false,
  templateUrl: './comment.component.html',
  styleUrl: './comment.component.scss'
})
export class CommentComponent implements OnInit {
  private readonly _profilesService = inject(ProfilesService);

  @Input() comment!: IComment;

  profileImage: SafeUrl | null = null;

  ngOnInit(): void {
    this.loadProfileImage()
      .subscribe(() => {})
  }

  private loadProfileImage(): Observable<any> {
    if (!this.comment.profile.hasImage)
      return of();

    return this._profilesService.getImage(this.comment.profile.id)
      .pipe(
        tap(response => this.profileImage = URL.createObjectURL(response)),
        map(response => undefined)
      )
  }
}
