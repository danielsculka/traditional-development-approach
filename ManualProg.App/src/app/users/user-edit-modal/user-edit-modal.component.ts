import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IUser } from '../../shared/models/user';
import { FormBuilder, Validators } from '@angular/forms';
import { UsersService } from '../data-access/users.service';
import { UserRole } from '../../shared/enums/user-role';
import { IUpdateUserRequest } from '../data-access/requests/update-user-request';

@Component({
  selector: 'app-user-edit-modal',
  standalone: false,
  templateUrl: './user-edit-modal.component.html',
  styleUrl: './user-edit-modal.component.scss'
})
export class UserEditModalComponent {
  private readonly _fb = inject(FormBuilder);
  private readonly _usersService = inject(UsersService);
  private readonly _dialogRef = inject(MatDialogRef<UserEditModalComponent>);

  user: IUser = inject(MAT_DIALOG_DATA);

  form = this._fb.nonNullable.group({
    username: [{ value: this.user?.username, disabled: !!this.user }, Validators.required],
    password: ['', Validators.required],
    role: [!!this.user ? this.user.role : UserRole.Moderator, Validators.required]
  });

  roles: UserRole[] = [UserRole.Administrator, UserRole.Moderator];

  save(): void {
    const user = this.form.getRawValue();

    if (!!this.user) {
      const data = <IUpdateUserRequest> {
        role: user.role
      }

      this._usersService.update(this.user.id, data)
        .subscribe(() => {
          this._dialogRef.close(true);
        });
    } else {
      this._usersService.create(user)
        .subscribe(() => {
          this._dialogRef.close(true);
        });
    }
  }

  close(): void {
    this._dialogRef.close();
  }
}
