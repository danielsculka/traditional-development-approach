import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PostsService } from '../data-access/posts.service';
import { ICreatePostRequest } from '../data-access/requests/create-post-request';
import { requiredFileType } from '../../shared/validators/required-file-type';

@Component({
  selector: 'app-posts-create',
  standalone: false,
  templateUrl: './posts-create.component.html',
  styleUrl: './posts-create.component.scss'
})
export class PostsCreateComponent {
  private readonly _fb = inject(FormBuilder);
  private readonly _service = inject(PostsService);
  private readonly _router = inject(Router);

  form = this._fb.nonNullable.group({
    isPublic: [true, Validators.required],
    price: [0, Validators.required],
    description: [''],
    images: [null, [Validators.required, requiredFileType('png')]]
  });

  onSubmit(): void {
    const values = this.form.getRawValue();

    const files: File[] = this.form.value?.images as unknown as File[];

    const request: ICreatePostRequest = {
      isPublic: values.isPublic,
      price: values.price,
      description: values.description,
      images: files
    };

    this._service.create(request).subscribe(response => {
      this._router.navigateByUrl('/profile');
    });
  }
}
