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

@NgModule({
  declarations: [
    PostsListComponent,
    PostsCreateComponent
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
