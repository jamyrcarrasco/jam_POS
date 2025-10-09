import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { ProductService } from '../../services/product.service';
import { Product, ProductFilter } from '../../models/product.model';
import { PagedResult } from '../../../../core/models/pagination.model';
import { CategoryService } from '../../../categories/services/category.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatTooltipModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatChipsModule,
    MatSortModule,
    MatDividerModule,
    MatExpansionModule
  ],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  pagedResult: PagedResult<Product> | null = null;
  productForm: FormGroup;
  filterForm: FormGroup;
  categorias: any[] = [];
  
  isEditing = false;
  editingId: number | null = null;
  displayedColumns: string[] = ['nombre', 'descripcion', 'precio', 'stock', 'categoriaNombre', 'activo', 'acciones'];
  loading = false;
  showFilters = false;

  // Pagination settings
  pageSize = 10;
  pageNumber = 1;
  pageSizeOptions = [5, 10, 25, 50, 100];

  // Sort settings
  sortBy = 'nombre';
  sortDescending = false;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private fb: FormBuilder,
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

    this.filterForm = this.fb.group({
      searchTerm: [''],
      categoria: [''],
      precioMin: [null],
      precioMax: [null],
      stockBajo: [false],
      activo: [null]
    });
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategorias();

    // Subscribe to filter changes
    this.filterForm.valueChanges.subscribe(() => {
      this.pageNumber = 1; // Reset to first page when filters change
      this.loadProducts();
    });
  }

  loadProducts(): void {
    this.loading = true;
    const filter: ProductFilter = {
      ...this.filterForm.value,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      orderBy: this.sortBy,
      orderDescending: this.sortDescending
    };

    this.productService.getProducts(filter).subscribe({
      next: (result) => {
        this.pagedResult = result;
        this.products = result.items;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.snackBar.open('Error al cargar los productos', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  loadCategorias(): void {
    this.categoryService.getActiveCategories().subscribe({
      next: (categorias) => {
        this.categorias = categorias;
      },
      error: (error) => {
        console.error('Error al cargar categorías:', error);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadProducts();
  }

  onSortChange(sort: Sort): void {
    this.sortBy = sort.active || 'nombre';
    this.sortDescending = sort.direction === 'desc';
    this.loadProducts();
  }

  clearFilters(): void {
    this.filterForm.reset({
      searchTerm: '',
      categoria: '',
      precioMin: null,
      precioMax: null,
      stockBajo: false,
      activo: null
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      const productData = this.productForm.value;
      
      if (this.isEditing && this.editingId) {
        this.updateProduct(productData);
      } else {
        this.createProduct(productData);
      }
    } else {
      this.snackBar.open('Por favor complete todos los campos requeridos', 'Cerrar', { duration: 3000 });
    }
  }

  private createProduct(productData: any): void {
    this.loading = true;
    this.productService.createProduct(productData).subscribe({
      next: () => {
        this.loadProducts();
        this.resetForm();
        this.snackBar.open('Producto creado exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error creating product:', error);
        this.snackBar.open('Error al crear el producto', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private updateProduct(productData: any): void {
    if (!this.editingId) return;
    
    this.loading = true;
    this.productService.updateProduct(this.editingId, productData).subscribe({
      next: () => {
        this.loadProducts();
        this.resetForm();
        this.snackBar.open('Producto actualizado exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error updating product:', error);
        this.snackBar.open('Error al actualizar el producto', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  editProduct(product: Product): void {
    this.isEditing = true;
    this.editingId = product.id;
    this.productForm.patchValue({
      nombre: product.nombre,
      descripcion: product.descripcion,
      precio: product.precio,
      stock: product.stock,
      categoriaId: product.categoriaId,
      codigoBarras: product.codigoBarras,
      imagenUrl: product.imagenUrl,
      precioCompra: product.precioCompra,
      margenGanancia: product.margenGanancia,
      stockMinimo: product.stockMinimo,
      activo: product.activo
    });

    // Scroll to form
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  deleteProduct(id: number): void {
    if (confirm('¿Está seguro de que desea eliminar este producto?')) {
      this.loading = true;
      this.productService.deleteProduct(id).subscribe({
        next: () => {
          this.loadProducts();
          this.snackBar.open('Producto eliminado exitosamente', 'Cerrar', { duration: 3000 });
          this.loading = false;
        },
        error: (error) => {
          console.error('Error deleting product:', error);
          this.snackBar.open('Error al eliminar el producto', 'Cerrar', { duration: 3000 });
          this.loading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.productForm.reset({ activo: true });
    this.isEditing = false;
    this.editingId = null;
  }

  cancelEdit(): void {
    this.resetForm();
  }

  getStockStatus(stock: number, stockMinimo?: number): string {
    if (stock === 0) return 'Sin stock';
    if (stockMinimo && stock <= stockMinimo) return 'Stock bajo';
    return 'En stock';
  }

  getStockStatusClass(stock: number, stockMinimo?: number): string {
    if (stock === 0) return 'stock-empty';
    if (stockMinimo && stock <= stockMinimo) return 'stock-low';
    return 'stock-ok';
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  getActiveFilterCount(): number {
    const filters = this.filterForm.value;
    let count = 0;
    if (filters.searchTerm) count++;
    if (filters.categoria) count++;
    if (filters.precioMin !== null) count++;
    if (filters.precioMax !== null) count++;
    if (filters.stockBajo) count++;
    if (filters.activo !== null) count++;
    return count;
  }
}
