import { Component, Input } from '@angular/core';
import { IPostResponse } from '../../data-access/responses/post-response';

@Component({
  selector: 'app-post-description',
  standalone: false,
  templateUrl: './post-description.component.html',
  styleUrl: './post-description.component.scss'
})
export class PostDescriptionComponent {
  @Input() post!: IPostResponse;
}
