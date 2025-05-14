import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-post-modal',
  standalone: false,
  templateUrl: './post-modal.component.html',
  styleUrl: './post-modal.component.scss'
})
export class PostModalComponent {
  private readonly _dialogRef = inject(MatDialogRef<PostModalComponent>);
  readonly post = inject(MAT_DIALOG_DATA);
}
