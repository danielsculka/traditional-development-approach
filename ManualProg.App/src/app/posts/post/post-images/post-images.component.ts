import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { PostsService } from '../../data-access/posts.service';
import { IPost } from '../post.component';
import { SafeUrl } from '@angular/platform-browser';
import { forkJoin, map, Observable, tap } from 'rxjs';

@Component({
  selector: 'app-post-images',
  standalone: false,
  templateUrl: './post-images.component.html',
  styleUrl: './post-images.component.scss'
})
export class PostImagesComponent implements OnInit  {
  private readonly _postsService = inject(PostsService);

  @Input() post!: IPost;
  @Output() onExpand: EventEmitter<any> = new EventEmitter<any>();

  postImages: { [id: string] : SafeUrl } = {};
  isIndependent: boolean = true;

  ngOnInit(): void {
    this.isIndependent = this.post.hasOwnProperty('profile');

    var imageLoad = this.isIndependent
      ? this.post.imageIds.map(id => this.loadPostImage(id))
      : [this.loadPostImage(this.post.imageIds[0])];

    forkJoin(imageLoad)
      .subscribe(() => {});
  }

  private loadPostImage(imageId: string): Observable<any> {
    return this._postsService.getImage(imageId)
      .pipe(
        tap(response => this.postImages[imageId] = URL.createObjectURL(response)),
        map(response => undefined)
      )
  }
}
