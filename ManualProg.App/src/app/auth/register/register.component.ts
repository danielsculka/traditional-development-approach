import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { requiredFileType } from '../../shared/validators/required-file-type';
import { IRegisterRequest, IRegisterRequestProfileData } from '../data-access/requests/register-request';
import { AuthService } from '../data-access/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly _fb = inject(FormBuilder);
  private readonly _service = inject(AuthService);
  private readonly _router = inject(Router);

  form = this._fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required],
    profile: this._fb.nonNullable.group({
      name: ['', Validators.required],
      description: [''],
      images: [null, [Validators.required, requiredFileType('png')]]
    })
  });

  onSubmit(): void {
    const values = this.form.getRawValue();

    const files: File[] = this.form.value.profile?.images as unknown as File[];

    const request: IRegisterRequest = {
      username: values.username,
      password: values.password,
      profile: <IRegisterRequestProfileData>{
        name: values.profile.name,
        description: values.profile.description,
        image: files[0]
      }
    };

    this._service.register(request).subscribe(response => {
      this._router.navigate(['/']);
    });
  }
}
