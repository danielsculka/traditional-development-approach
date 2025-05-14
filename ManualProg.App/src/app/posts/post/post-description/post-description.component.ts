import { Component, Input, OnInit } from '@angular/core';
import { IPost } from '../post.component';
import { IPostResponse, IPostResponseProfileData } from '../../data-access/responses/post-response';

@Component({
  selector: 'app-post-description',
  standalone: false,
  templateUrl: './post-description.component.html',
  styleUrl: './post-description.component.scss'
})
export class PostDescriptionComponent implements OnInit  {
  @Input() post!: IPost;

  profile!: IPostResponseProfileData;

  ngOnInit(): void {
    const hasProfile = this.post.hasOwnProperty('profile');

    if (hasProfile) {
      const p = this.post as IPostResponse;

      this.profile = p.profile;
    }
  }
}
