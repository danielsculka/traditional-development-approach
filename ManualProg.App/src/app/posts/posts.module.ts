import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { SharedModule } from '../shared/shared.module';
import { PostsRoutingModule } from './posts-routing.module';
import { PostsListComponent } from './posts-list/posts-list.component';
import { PostsCreateComponent } from './posts-create/posts-create.component';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { PostComponent } from './post/post.component';
import { PostActionsComponent } from './post/post-actions/post-actions.component';
import { PostHeaderComponent } from './post/post-header/post-header.component';
import { PostDescriptionComponent } from './post/post-description/post-description.component';
import { PostModalComponent } from './post/post-modal/post-modal.component';
import { PostImagesComponent } from './post/post-images/post-images.component';

@NgModule({
  declarations: [
    PostsListComponent,
    PostsCreateComponent,
    PostComponent,
    PostActionsComponent,
    PostHeaderComponent,
    PostDescriptionComponent,
    PostModalComponent,
    PostImagesComponent
  ],
  exports: [
    PostComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    PostsRoutingModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    MatInputModule,
    ReactiveFormsModule,
    MatSlideToggleModule
  ]
})
export class PostsModule { }
