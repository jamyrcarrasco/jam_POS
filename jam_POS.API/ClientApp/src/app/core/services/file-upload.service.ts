import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpEventType, HttpProgressEvent } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface FileUploadResponse {
  success: boolean;
  fileName?: string;
  fileUrl?: string;
  filePath?: string;
  fileSize?: number;
  contentType?: string;
  errorMessage?: string;
}

export interface UploadProgress {
  loaded: number;
  total: number;
  percentage: number;
}

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private apiUrl: string;
  private uploadProgressSubject = new BehaviorSubject<UploadProgress>({ loaded: 0, total: 0, percentage: 0 });

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}fileupload`;
  }

  /**
   * Upload an image file to R2 storage
   * @param file The file to upload
   * @param folder Optional folder name (default: products)
   * @returns Observable with upload response
   */
  uploadImage(file: File, folder: string = 'products'): Observable<FileUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('folder', folder);

    return this.http.post<FileUploadResponse>(`${this.apiUrl}/upload-image`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map((event: HttpEvent<any>) => {
        if (event.type === HttpEventType.UploadProgress) {
          const progress = event as HttpProgressEvent;
          const percentage = progress.total ? Math.round(100 * progress.loaded / progress.total) : 0;
          this.uploadProgressSubject.next({
            loaded: progress.loaded || 0,
            total: progress.total || 0,
            percentage
          });
        } else if (event.type === HttpEventType.Response) {
          return event.body;
        }
        return null;
      }),
      map(response => response as FileUploadResponse),
      catchError(error => {
        console.error('Upload error:', error);
        throw error;
      })
    );
  }

  /**
   * Delete a file from R2 storage
   * @param filePath The path of the file to delete
   * @returns Observable with deletion result
   */
  deleteFile(filePath: string): Observable<{ success: boolean }> {
    return this.http.delete<{ success: boolean }>(`${this.apiUrl}/delete`, {
      params: { filePath }
    });
  }

  /**
   * Get the public URL for a file
   * @param filePath The file path
   * @returns Observable with public URL
   */
  getFileUrl(filePath: string): Observable<string> {
    return this.http.get<string>(`${this.apiUrl}/url`, {
      params: { filePath }
    });
  }

  /**
   * Get upload progress observable
   * @returns Observable with upload progress
   */
  getUploadProgress(): Observable<UploadProgress> {
    return this.uploadProgressSubject.asObservable();
  }

  /**
   * Reset upload progress
   */
  resetProgress(): void {
    this.uploadProgressSubject.next({ loaded: 0, total: 0, percentage: 0 });
  }

  /**
   * Validate file before upload
   * @param file The file to validate
   * @returns Validation result
   */
  validateFile(file: File): { valid: boolean; error?: string } {
    const maxSize = 10 * 1024 * 1024; // 10MB
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/webp', 'image/gif'];
    const allowedExtensions = ['.jpg', '.jpeg', '.png', '.webp', '.gif'];

    // Check file size
    if (file.size > maxSize) {
      return { valid: false, error: 'El archivo es demasiado grande. Máximo 10MB.' };
    }

    // Check file type
    if (!allowedTypes.includes(file.type)) {
      return { valid: false, error: 'Tipo de archivo no permitido. Solo se permiten imágenes.' };
    }

    // Check file extension
    const extension = this.getFileExtension(file.name);
    if (!allowedExtensions.includes(extension)) {
      return { valid: false, error: 'Extensión de archivo no permitida.' };
    }

    return { valid: true };
  }

  /**
   * Get file extension from filename
   * @param filename The filename
   * @returns File extension in lowercase
   */
  private getFileExtension(filename: string): string {
    return filename.toLowerCase().substring(filename.lastIndexOf('.'));
  }

  /**
   * Generate a preview URL for an image file
   * @param file The image file
   * @returns Observable with preview URL
   */
  generatePreviewUrl(file: File): Observable<string> {
    return new Observable(observer => {
      const reader = new FileReader();
      reader.onload = (e) => {
        observer.next(e.target?.result as string);
        observer.complete();
      };
      reader.onerror = (error) => {
        observer.error(error);
      };
      reader.readAsDataURL(file);
    });
  }
}
