import { Component, Inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SpaceService } from '../space.service';
import { CreateSpaceRequest, SpaceDto, UpdateSpaceRequest } from '../space.model';
import { StorageSelector } from '../../../shared/components/storage-selector/storage-selector';

export interface SpaceEditorData {
  space?: SpaceDto;
  mode: 'create' | 'edit';
}

@Component({
  selector: 'app-space-editor',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    StorageSelector
  ],
  providers: [SpaceService],
  templateUrl: './space-editor.html',
  styleUrl: './space-editor.scss',
})
export class SpaceEditor implements OnInit {
  public spaceForm: FormGroup;
  public loading = signal(false);
  public isEditMode = false;
  public spaceId?: string;

  constructor(
    private fb: FormBuilder,
    private spaceService: SpaceService,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<SpaceEditor>,
    @Inject(MAT_DIALOG_DATA) public data: SpaceEditorData
  ) {
    this.isEditMode = data.mode === 'edit';
    this.spaceId = data.space?.id;

    this.spaceForm = this.fb.group({
      name: [data.space?.name || '', [Validators.required, Validators.maxLength(100)]],
      description: [data.space?.description || '', [Validators.maxLength(500)]],
      storageId: [data.space?.storageId || '', [Validators.required]]
    });
  }

  ngOnInit() {
    // Space data is already loaded from dialog data
  }

  onSubmit() {
    if (this.spaceForm.invalid) {
      this.spaceForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const formValue = this.spaceForm.value;

    if (this.isEditMode && this.spaceId) {
      const request: UpdateSpaceRequest = {
        name: formValue.name,
        description: formValue.description || undefined
      };

      this.spaceService.update(this.spaceId, request).subscribe({
        next: (result) => {
          this.loading.set(false);
          this.snackBar.open('Space updated successfully', 'Close', { duration: 3000 });
          this.dialogRef.close(result);
        },
        error: (error) => {
          this.loading.set(false);
          this.snackBar.open('Error updating space: ' + (error.message || 'Unknown error'), 'Close', { duration: 5000 });
        }
      });
    } else {
      const request: CreateSpaceRequest = {
        name: formValue.name,
        description: formValue.description || undefined,
        storageId: formValue.storageId
      };

      this.spaceService.create(request).subscribe({
        next: (result) => {
          this.loading.set(false);
          this.snackBar.open('Space created successfully', 'Close', { duration: 3000 });
          this.dialogRef.close(result);
        },
        error: (error) => {
          this.loading.set(false);
          this.snackBar.open('Error creating space: ' + (error.message || 'Unknown error'), 'Close', { duration: 5000 });
        }
      });
    }
  }

  onCancel() {
    this.dialogRef.close();
  }

  getErrorMessage(fieldName: string): string {
    const field = this.spaceForm.get(fieldName);
    if (field?.hasError('required')) {
      return `${fieldName} is required`;
    }
    if (field?.hasError('maxLength')) {
      return `${fieldName} is too long`;
    }
    return '';
  }
}
