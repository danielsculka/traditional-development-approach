import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ILoginRequest } from '../data-access/requests/login-request';
import { AuthService } from '../data-access/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly _fb = inject(FormBuilder);
  private readonly _service = inject(AuthService);
  private readonly _router = inject(Router);

  form = this._fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  onSubmit(): void {
    const request: ILoginRequest = this.form.getRawValue();

    this._service.login(request).subscribe(response => {
      this._router.navigate(['/']);
    });
  }
}
