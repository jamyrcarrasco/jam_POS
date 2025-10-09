import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
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
import { CategoryService } from '../../services/category.service';
import { Category, CategoryFilter } from '../../models/category.model';
import { PagedResult } from '../../../../core/models/pagination.model';

@Component({
  selector: 'app-category-list',
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
    MatDividerModule
  ],
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.scss']
})
export class CategoryListComponent implements OnInit {
  categories: Category[] = [];
  pagedResult: PagedResult<Category> | null = null;
  categoryForm: FormGroup;
  filterForm: FormGroup;
  
  isEditing = false;
  editingId: number | null = null;
  displayedColumns: string[] = ['nombre', 'descripcion', 'color', 'productosCount', 'activo', 'acciones'];
  loading = false;
  showForm = false;

  // Pagination settings
  pageSize = 10;
  pageNumber = 1;
  pageSizeOptions = [5, 10, 25, 50];

  // Sort settings
  sortBy = 'nombre';
  sortDescending = false;

  // Colores predefinidos para las categorías
  predefinedColors = [
    { value: '#3B82F6', name: 'Azul' },
    { value: '#10B981', name: 'Verde' },
    { value: '#8B5CF6', name: 'Morado' },
    { value: '#F59E0B', name: 'Naranja' },
    { value: '#EC4899', name: 'Rosa' },
    { value: '#EF4444', name: 'Rojo' },
    { value: '#14B8A6', name: 'Turquesa' },
    { value: '#6366F1', name: 'Índigo' }
  ];

  // Iconos predefinidos (Material Icons)
  predefinedIcons = [
    'local_drink', 'restaurant', 'devices', 'cleaning_services', 'checkroom',
    'shopping_cart', 'home', 'sports', 'book', 'toys', 'pets', 'local_florist',
    'build', 'kitchen', 'chair', 'weekend', 'directions_car', 'computer'
  ];

  constructor(
    private categoryService: CategoryService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.categoryForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.maxLength(500)]],
      color: ['#3B82F6'],
      icono: ['category'],
      activo: [true]
    });

    this.filterForm = this.fb.group({
      searchTerm: [''],
      activo: [null]
    });
  }

  ngOnInit(): void {
    this.loadCategories();

    // Subscribe to filter changes
    this.filterForm.valueChanges.subscribe(() => {
      this.pageNumber = 1; // Reset to first page when filters change
      this.loadCategories();
    });
  }

  loadCategories(): void {
    this.loading = true;
    const filter: CategoryFilter = {
      ...this.filterForm.value,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      orderBy: this.sortBy,
      orderDescending: this.sortDescending
    };

    this.categoryService.getCategories(filter).subscribe({
      next: (result) => {
        this.pagedResult = result;
        this.categories = result.items;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar categorías:', error);
        this.snackBar.open('Error al cargar las categorías', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadCategories();
  }

  onSortChange(sort: Sort): void {
    this.sortBy = sort.active || 'nombre';
    this.sortDescending = sort.direction === 'desc';
    this.loadCategories();
  }

  clearFilters(): void {
    this.filterForm.reset({
      searchTerm: '',
      activo: null
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.resetForm();
    }
  }

  onSubmit(): void {
    if (this.categoryForm.valid) {
      const categoryData = this.categoryForm.value;
      
      if (this.isEditing && this.editingId) {
        this.updateCategory(categoryData);
      } else {
        this.createCategory(categoryData);
      }
    } else {
      this.snackBar.open('Por favor complete todos los campos requeridos', 'Cerrar', { duration: 3000 });
    }
  }

  private createCategory(categoryData: any): void {
    this.loading = true;
    this.categoryService.createCategory(categoryData).subscribe({
      next: () => {
        this.loadCategories();
        this.resetForm();
        this.showForm = false;
        this.snackBar.open('Categoría creada exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al crear categoría:', error);
        this.snackBar.open('Error al crear la categoría', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private updateCategory(categoryData: any): void {
    if (!this.editingId) return;
    
    this.loading = true;
    this.categoryService.updateCategory(this.editingId, categoryData).subscribe({
      next: () => {
        this.loadCategories();
        this.resetForm();
        this.showForm = false;
        this.snackBar.open('Categoría actualizada exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al actualizar categoría:', error);
        this.snackBar.open('Error al actualizar la categoría', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  editCategory(category: Category): void {
    this.isEditing = true;
    this.editingId = category.id;
    this.showForm = true;
    this.categoryForm.patchValue({
      nombre: category.nombre,
      descripcion: category.descripcion,
      color: category.color || '#3B82F6',
      icono: category.icono || 'category',
      activo: category.activo
    });

    // Scroll to form
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  deleteCategory(id: number): void {
    if (confirm('¿Está seguro de que desea eliminar esta categoría?')) {
      this.loading = true;
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          this.loadCategories();
          this.snackBar.open('Categoría eliminada exitosamente', 'Cerrar', { duration: 3000 });
          this.loading = false;
        },
        error: (error) => {
          console.error('Error al eliminar categoría:', error);
          const errorMsg = error.error?.message || 'Error al eliminar la categoría';
          this.snackBar.open(errorMsg, 'Cerrar', { duration: 5000 });
          this.loading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.categoryForm.reset({ 
      activo: true,
      color: '#3B82F6',
      icono: 'category'
    });
    this.isEditing = false;
    this.editingId = null;
  }

  cancelEdit(): void {
    this.resetForm();
    this.showForm = false;
  }

  getActiveFilterCount(): number {
    const filters = this.filterForm.value;
    let count = 0;
    if (filters.searchTerm) count++;
    if (filters.activo !== null) count++;
    return count;
  }
}

