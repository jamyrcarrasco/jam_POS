import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FileUploadService, FileUploadResponse, UploadProgress } from '../../../core/services/file-upload.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-image-upload',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatSnackBarModule,
    MatCardModule,
    MatTooltipModule
  ],
  templateUrl: './image-upload.component.html',
  styleUrls: ['./image-upload.component.scss']
})
export class ImageUploadComponent implements OnInit, OnDestroy {
  @Input() control: AbstractControl = new FormControl();
  @Input() folder: string = 'products';
  @Input() disabled: boolean = false;
  
  @Output() imageUploaded = new EventEmitter<FileUploadResponse>();
  @Output() imageRemoved = new EventEmitter<void>();

  previewUrl: string | null = null;
  uploadedFile: FileUploadResponse | null = null;
  isUploading: boolean = false;
  isDragOver: boolean = false;
  uploadProgress: UploadProgress = { loaded: 0, total: 0, percentage: 0 };

  private destroy$ = new Subject<void>();

  constructor(
    private fileUploadService: FileUploadService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    // Initialize with existing value if control has one
    if (this.control.value) {
      this.previewUrl = this.control.value;
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;

    if (this.disabled || this.isUploading) return;

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.handleFile(files[0]);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.handleFile(input.files[0]);
    }
  }

  private handleFile(file: File): void {
    // Validate file
    const validation = this.fileUploadService.validateFile(file);
    if (!validation.valid) {
      this.snackBar.open(validation.error || 'Archivo inválido', 'Cerrar', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
      return;
    }

    // Generate preview
    this.fileUploadService.generatePreviewUrl(file)
      .pipe(takeUntil(this.destroy$))
      .subscribe((previewUrl: string) => {
        this.previewUrl = previewUrl;
      });

    // Upload file
    this.uploadFile(file);
  }

  private uploadFile(file: File): void {
    this.isUploading = true;
    this.uploadProgress = { loaded: 0, total: 0, percentage: 0 };

    // Subscribe to upload progress
    this.fileUploadService.getUploadProgress()
      .pipe(takeUntil(this.destroy$))
      .subscribe((progress: UploadProgress) => {
        this.uploadProgress = progress;
      });

    // Upload file
    this.fileUploadService.uploadImage(file, this.folder)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: FileUploadResponse) => {
          this.isUploading = false;
          this.uploadedFile = response;
          this.control.setValue(response.fileUrl);
          this.imageUploaded.emit(response);
          
          this.snackBar.open('Imagen subida exitosamente', 'Cerrar', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
        },
        error: (error: any) => {
          this.isUploading = false;
          this.previewUrl = null;
          console.error('Upload error:', error);
          
          this.snackBar.open('Error al subir la imagen', 'Cerrar', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }


  removeImage(event: Event): void {
    event.stopPropagation();
    
    if (this.uploadedFile?.filePath) {
      // Delete from server
      this.fileUploadService.deleteFile(this.uploadedFile.filePath)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.resetComponent();
            this.imageRemoved.emit();
          },
          error: (error: any) => {
            console.error('Delete error:', error);
            // Still reset locally even if server delete fails
            this.resetComponent();
            this.imageRemoved.emit();
          }
        });
    } else {
      this.resetComponent();
      this.imageRemoved.emit();
    }
  }

  private resetComponent(): void {
    this.previewUrl = null;
    this.uploadedFile = null;
    this.control.setValue(null);
    this.fileUploadService.resetProgress();
  }

  copyUrl(): void {
    if (this.uploadedFile?.fileUrl) {
      navigator.clipboard.writeText(this.uploadedFile.fileUrl).then(() => {
        this.snackBar.open('URL copiada al portapapeles', 'Cerrar', {
          duration: 2000
        });
      });
    }
  }
}
