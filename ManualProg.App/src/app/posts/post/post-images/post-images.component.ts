import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { PostsService } from '../../data-access/posts.service';
import { SafeUrl } from '@angular/platform-browser';
import { forkJoin, map, Observable, tap } from 'rxjs';
import { IPostResponse } from '../../data-access/responses/post-response';

@Component({
  selector: 'app-post-images',
  standalone: false,
  templateUrl: './post-images.component.html',
  styleUrl: './post-images.component.scss'
})
export class PostImagesComponent implements OnInit  {
  private readonly _postsService = inject(PostsService);

  @Input() post!: IPostResponse;
  @Input() isMinimalistic: boolean = true;
  @Output() onExpand: EventEmitter<any> = new EventEmitter<any>();
  @Output() onPurchase: EventEmitter<any> = new EventEmitter<any>();

  postImages: { [id: string] : SafeUrl } = {};

  ngOnInit(): void {
    if (this.post.hasAccess) {
      var imageLoad = this.isMinimalistic
        ? this.post.imageIds.map(id => this.loadPostImage(id))
        : [this.loadPostImage(this.post.imageIds[0])];

      forkJoin(imageLoad)
        .subscribe(() => {});
    }
  }

  private loadPostImage(imageId: string): Observable<any> {
    return this._postsService.getImage(imageId)
      .pipe(
        tap(response => this.postImages[imageId] = URL.createObjectURL(response)),
        map(response => undefined)
      )
  }
}
