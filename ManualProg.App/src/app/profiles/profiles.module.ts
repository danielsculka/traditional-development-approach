import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { SharedModule } from '../shared/shared.module';
import { MatTableModule } from '@angular/material/table';
import { ProfileComponent } from './profile/profile.component';
import { ProfilesRoutingModule } from './profiles-routing.module';
import { MatTabsModule } from '@angular/material/tabs';

@NgModule({
  declarations: [
    ProfileComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    ProfilesRoutingModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    MatInputModule,
    ReactiveFormsModule,
    MatTableModule,
    MatTabsModule
  ]
})
export class ProfilesModule { }
