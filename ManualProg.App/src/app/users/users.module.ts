import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { SharedModule } from '../shared/shared.module';
import { UsersRoutingModule } from './users-routing.module';
import { UsersListComponent } from './users-list/users-list.component';
import { MatTableModule } from '@angular/material/table';

@NgModule({
  declarations: [
    UsersListComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    UsersRoutingModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    MatInputModule,
    ReactiveFormsModule,
    MatTableModule
  ]
})
export class UsersModule { }
