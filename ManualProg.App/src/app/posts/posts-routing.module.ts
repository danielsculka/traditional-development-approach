import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PostsListComponent } from './posts-list/posts-list.component';
import { PostsCreateComponent } from './posts-create/posts-create.component';
import { UserRoleGuard } from '../guards/user-role.guard';
import { UserRole } from '../shared/enums/user-role';

const routes: Routes = [
  {
    path: 'create',
    component: PostsCreateComponent,
    canActivate: [UserRoleGuard],
    data: { roles: [UserRole.Basic] }
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
