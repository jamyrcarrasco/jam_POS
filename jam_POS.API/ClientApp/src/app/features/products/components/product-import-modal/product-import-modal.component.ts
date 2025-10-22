import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProductService } from '../../services/product.service';
import { ProductImportResult } from '../../models/product-import-result.model';

@Component({
  selector: 'app-product-import-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './product-import-modal.component.html',
  styleUrls: ['./product-import-modal.component.scss']
})
export class ProductImportModalComponent {
  selectedFile: File | null = null;
  uploadInProgress = false;
  downloadInProgress = false;
  importResult: ProductImportResult | null = null;

  constructor(
    private dialogRef: MatDialogRef<ProductImportModalComponent>,
    private productService: ProductService,
    private snackBar: MatSnackBar
  ) {
    this.dialogRef.disableClose = true;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.importResult = null;
    }
  }

  downloadTemplate(): void {
    this.downloadInProgress = true;
    this.productService.downloadTemplate().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `plantilla_productos_${new Date().getTime()}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.downloadInProgress = false;
      },
      error: (error) => {
        console.error('Error downloading template:', error);
        this.downloadInProgress = false;
        this.snackBar.open('Error al descargar la plantilla', 'Cerrar', {
          duration: 4000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  onUpload(): void {
    if (!this.selectedFile) {
      this.snackBar.open('Selecciona un archivo para importar', 'Cerrar', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
      return;
    }

    this.uploadInProgress = true;
    this.importResult = null;

    this.productService.importProducts(this.selectedFile).subscribe({
      next: (result) => {
        this.uploadInProgress = false;
        this.importResult = result;
        const hasErrors = result.failedCount > 0;
        this.snackBar.open(
          hasErrors ? 'Importación completada con observaciones' : 'Productos importados correctamente',
          'Cerrar',
          {
            duration: 4000,
            panelClass: [hasErrors ? 'error-snackbar' : 'success-snackbar']
          }
        );
      },
      error: (error) => {
        console.error('Error importing products:', error);
        this.uploadInProgress = false;
        this.snackBar.open('Error al importar los productos', 'Cerrar', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  onCancel(): void {
    if (this.uploadInProgress) {
      return;
    }
    this.dialogRef.close({ success: false });
  }

  onFinish(): void {
    if (this.importResult) {
      this.dialogRef.close({ success: true, summary: this.importResult });
    } else {
      this.dialogRef.close({ success: false });
    }
  }
}
