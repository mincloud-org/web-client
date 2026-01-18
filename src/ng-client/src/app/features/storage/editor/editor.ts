import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute, Router } from '@angular/router';
import { StorageService } from '../storage.service';
import { StorageDto, CreateStorageRequest } from '../storage.model';

@Component({
  selector: 'app-storage-editor',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatSelectModule,
    MatCardModule
  ],
  templateUrl: './editor.html',
  styleUrl: './editor.scss',
})
export class StorageEditor implements OnInit {
  public form?: FormGroup;
  public loading = signal(false);
  public isEditMode = false;
  public id?: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private storageService: StorageService,
    private snackBar: MatSnackBar
  ) {
    this.id = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = this.route.snapshot.routeConfig?.path === 'edit/:id';
  }

  ngOnInit() {
    if (this.isEditMode && this.id) {
      this.loading.set(true);
      this.storageService.getById(this.id).subscribe({
        next: (data) => {
          this.buildEditForm(data);
          this.loading.set(false);
        },
        error: () => {
          this.snackBar.open('Failed to load storage data', 'Close', { duration: 3000 });
          this.loading.set(false);
        }
      });
    } else {
      this.buildCreateForm();
    }
  }

  private buildCreateForm() {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      type: ['awsS3', [Validators.required]],
      credentialsJson: this.buildS3Form()
    });
    this.form.get('type')?.valueChanges.subscribe(type => {
      if (type === 'awsS3') {
        this.form?.setControl('credentialsJson', this.buildS3Form());
      } else if (type === 'azureBlob') {
        this.form?.setControl('credentialsJson', this.buildAzureBlobForm());
      }
    });
  }

  private buildEditForm(data: StorageDto) {
    // In edit mode, credentials are optional - only update if user provides new credentials
    const credentialsForm = data?.type === 'azureBlob' ? this.buildAzureBlobForm(false) : this.buildS3Form(false);
    
    this.form = this.fb.group({
      name: [data?.name || '', [Validators.required, Validators.maxLength(100)]],
      description: [data?.description || '', [Validators.maxLength(500)]],
      type: [data?.type || 'awsS3', [Validators.required]],
      credentialsJson: credentialsForm
    });
  }

  private buildS3Form(required = true) {
    const validators = required ? [Validators.required] : [];
    return this.fb.group({
      accessKeyId: ['', validators],
      secretAccessKey: ['', validators],
      region: ['', validators],
      bucketName: ['', validators],
      endpoint: [''],
      s3ForcePathStyle: [false]
    })
  }

  private buildAzureBlobForm(required = true) {
    const validators = required ? [Validators.required] : [];
    return this.fb.group({
      connectionString: ['', validators]
    })
  }

  onSubmit() {
    if (this.form?.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const formValue = this.form!.value;
    const typeValue = this.form!.get('type')?.value;

    if (this.isEditMode && this.id) {
      // In edit mode, check if credentials have been provided
      const credentialsJson = formValue.credentialsJson;
      const hasCredentials = this.hasValidCredentials(credentialsJson, typeValue);

      const request: CreateStorageRequest = {
        type: typeValue,
        name: formValue.name,
        description: formValue.description || undefined,
        credentialsJson: hasCredentials ? credentialsJson : undefined as any
      };

      this.storageService.update(this.id, request).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Storage updated successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/settings/storage/list']);
        },
        error: (error) => {
          this.loading.set(false);
          this.snackBar.open('Error updating storage: ' + (error.error?.message || error.message || 'Unknown error'), 'Close', { duration: 5000 });
        }
      });
    } else {
      const createRequest: CreateStorageRequest = {
        type: typeValue,
        name: formValue.name,
        description: formValue.description || undefined,
        credentialsJson: formValue.credentialsJson
      };

      this.storageService.create(createRequest).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Storage created successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/settings/storage/list']);
        },
        error: (error) => {
          this.loading.set(false);
          this.snackBar.open('Error creating storage: ' + (error.error?.message || error.message || 'Unknown error'), 'Close', { duration: 5000 });
        }
      });
    }
  }

  onCancel() {
    this.router.navigate(['/settings/storage/list']);
  }

  get selectedType() {
    return this.form?.get('type')?.value;
  }

  private hasValidCredentials(credentials: any, type: string): boolean {
    if (!credentials) return false;
    
    if (type === 'awsS3') {
      return !!(credentials.accessKeyId && credentials.secretAccessKey && 
                credentials.region && credentials.bucketName);
    } else if (type === 'azureBlob') {
      return !!(credentials.connectionString);
    }
    
    return false;
  }
}
