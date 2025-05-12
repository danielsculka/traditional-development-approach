import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PostsListComponent } from './posts-list/posts-list.component';
import { PostsCreateComponent } from './posts-create/posts-create.component';

const routes: Routes = [
  {
    path: 'create',
    component: PostsCreateComponent,
  },
  {
    path: '',
    component: PostsListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PostsRoutingModule { }
