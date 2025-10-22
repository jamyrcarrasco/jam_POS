import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';
import { CategoryService } from '../../../categories/services/category.service';
import { ImageUploadComponent } from '../../../../shared/components/image-upload/image-upload.component';
import { FileUploadResponse } from '../../../../core/services/file-upload.service';
import { Subject, takeUntil } from 'rxjs';

export interface ProductModalData {
  product?: Product;
  isEdit: boolean;
}

@Component({
  selector: 'app-product-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatDividerModule,
    ImageUploadComponent
  ],
  templateUrl: './product-modal.component.html',
  styleUrls: ['./product-modal.component.scss']
})
export class ProductModalComponent implements OnInit, OnDestroy {
  productForm: FormGroup;
  categorias: any[] = [];
  loading = false;
  private destroy$ = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<ProductModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductModalData,
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private snackBar: MatSnackBar
  ) {
    this.productForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(200)]],
      descripcion: ['', [Validators.maxLength(500)]],
      precio: [0, [Validators.required, Validators.min(0.01)]],
      stock: [0, [Validators.required, Validators.min(0)]],
      categoriaId: [null],
      codigoBarras: [''],
      imagenUrl: [''],
      precioCompra: [0, [Validators.min(0)]],
      margenGanancia: [0, [Validators.min(0), Validators.max(100)]],
      stockMinimo: [0, [Validators.min(0)]],
      activo: [true]
    });
  }

  ngOnInit(): void {
    this.loadCategorias();
    
    if (this.data.isEdit && this.data.product) {
      this.loadProductData();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCategorias(): void {
    this.categoryService.getActiveCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categorias: any[]) => {
          this.categorias = categorias || [];
        },
        error: (error: any) => {
          console.error('Error loading categories:', error);
          this.snackBar.open('Error al cargar las categorías', 'Cerrar', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }

  private loadProductData(): void {
    if (this.data.product) {
      this.productForm.patchValue({
        nombre: this.data.product.nombre,
        descripcion: this.data.product.descripcion,
        precio: this.data.product.precio,
        stock: this.data.product.stock,
        categoriaId: this.data.product.categoriaId,
        codigoBarras: this.data.product.codigoBarras,
        imagenUrl: this.data.product.imagenUrl,
        precioCompra: this.data.product.precioCompra,
        margenGanancia: this.data.product.margenGanancia,
        stockMinimo: this.data.product.stockMinimo,
        activo: this.data.product.activo
      });
    }
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading = true;
      const formData = this.productForm.value;

      if (this.data.isEdit && this.data.product) {
        this.updateProduct(formData);
      } else {
        this.createProduct(formData);
      }
    }
  }

  private createProduct(formData: any): void {
    this.productService.createProduct(formData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.loading = false;
          this.snackBar.open('Producto creado exitosamente', 'Cerrar', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.dialogRef.close({ success: true, product: response });
        },
        error: (error) => {
          this.loading = false;
          console.error('Error creating product:', error);
          this.snackBar.open('Error al crear el producto', 'Cerrar', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }

  private updateProduct(formData: any): void {
    if (!this.data.product?.id) return;

    this.productService.updateProduct(this.data.product.id, formData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.loading = false;
          this.snackBar.open('Producto actualizado exitosamente', 'Cerrar', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.dialogRef.close({ success: true, product: response });
        },
        error: (error) => {
          this.loading = false;
          console.error('Error updating product:', error);
          this.snackBar.open('Error al actualizar el producto', 'Cerrar', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }

  onImageUploaded(response: FileUploadResponse): void {
    // The form control is already updated by the image upload component
  }

  onImageRemoved(): void {
    // The form control is already updated by the image upload component
  }

  onCancel(): void {
    this.dialogRef.close({ success: false });
  }
}
