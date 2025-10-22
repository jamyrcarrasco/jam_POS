import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
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
import { ProductModalComponent, ProductModalData } from '../product-modal/product-modal.component';
import { ProductImportModalComponent } from '../product-import-modal/product-import-modal.component';
import { ProductImportResult } from '../../models/product-import-result.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
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
  filterForm: FormGroup;
  categorias: any[] = [];
  displayedColumns: string[] = ['imagen', 'nombre', 'descripcion', 'precio', 'stock', 'categoriaNombre', 'activo', 'acciones'];
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
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
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

  openProductModal(product?: Product): void {
    const dialogData: ProductModalData = {
      product: product,
      isEdit: !!product
    };

    const dialogRef = this.dialog.open(ProductModalComponent, {
      data: dialogData,
      width: '800px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.success) {
        this.loadProducts();
      }
    });
  }

  editProduct(product: Product): void {
    this.openProductModal(product);
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

  openImportModal(): void {
    const dialogRef = this.dialog.open(ProductImportModalComponent, {
      width: '520px',
      maxWidth: '95vw'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.success) {
        const summary: ProductImportResult | undefined = result.summary;
        if (summary) {
          const messageParts = [
            `Nuevos: ${summary.createdCount}`,
            `Actualizados: ${summary.updatedCount}`
          ];
          if (summary.failedCount > 0) {
            messageParts.push(`Errores: ${summary.failedCount}`);
          }
          this.snackBar.open(`Importación completada. ${messageParts.join(' · ')}`, 'Cerrar', {
            duration: 5000,
            panelClass: [summary.failedCount > 0 ? 'error-snackbar' : 'success-snackbar']
          });
        }
        this.loadProducts();
      }
    });
  }

  downloadTemplate(): void {
    this.productService.downloadTemplate().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `plantilla_productos_${new Date().getTime()}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.snackBar.open('Plantilla descargada correctamente', 'Cerrar', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
      },
      error: (error) => {
        console.error('Error downloading template:', error);
        this.snackBar.open('Error al descargar la plantilla', 'Cerrar', {
          duration: 4000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  onImageError(event: any): void {
    // Hide the broken image and show the no-image placeholder
    event.target.style.display = 'none';
    const parent = event.target.parentElement;
    const noImageDiv = parent.querySelector('.no-image');
    if (noImageDiv) {
      noImageDiv.style.display = 'flex';
    }
  }

}
