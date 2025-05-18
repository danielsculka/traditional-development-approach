import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { FileUploadInputCompontent } from './components/file-upload-input/file-upload-input.component';
import { ProfileTagComponent } from './components/profile-tag/profile-tag.component';

@NgModule({
  declarations: [
    FileUploadInputCompontent,
    ProfileTagComponent
  ],
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    MatInputModule,
    ReactiveFormsModule
  ],
  exports: [
    FileUploadInputCompontent,
    ProfileTagComponent
  ]
})
export class SharedModule { }
