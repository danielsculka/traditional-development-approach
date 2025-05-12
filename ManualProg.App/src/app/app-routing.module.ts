import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then((m) => m.AuthModule),
  },
  {
    path: 'users',
    loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
  },
  {
    path: 'posts',
    loadChildren: () => import('./posts/posts.module').then((m) => m.PostsModule),
  },
  {
    path: 'profiles',
    loadChildren: () => import('./profiles/profiles.module').then((m) => m.ProfilesModule),
  },
  {
    path: '**',
    redirectTo: 'posts'
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
