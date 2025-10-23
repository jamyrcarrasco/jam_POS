import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subject, takeUntil } from 'rxjs';
import { ReportsService } from '../../../reports/services/reports.service';
import { SalesReport, SalesReportFilter } from '../../../reports/models/sales-report.model';
import { Category } from '../../../categories/models/category.model';
import { CategoryService } from '../../../categories/services/category.service';
import { Product } from '../../../products/models/product.model';
import { ProductService } from '../../../products/services/product.service';

interface MonthOption {
  value: number;
  label: string;
}

@Component({
  selector: 'app-sales-report',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatChipsModule,
    MatTooltipModule
  ],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'es-ES' }
  ],
  templateUrl: './sales-report.component.html',
  styleUrls: ['./sales-report.component.scss']
})
export class SalesReportComponent implements OnInit, OnDestroy {
  filterForm: FormGroup;
  report: SalesReport | null = null;
  loading = false;
  errorMessage = '';
  currentFilterDescription = '';

  readonly months: MonthOption[] = [
    { value: 1, label: 'Enero' },
    { value: 2, label: 'Febrero' },
    { value: 3, label: 'Marzo' },
    { value: 4, label: 'Abril' },
    { value: 5, label: 'Mayo' },
    { value: 6, label: 'Junio' },
    { value: 7, label: 'Julio' },
    { value: 8, label: 'Agosto' },
    { value: 9, label: 'Septiembre' },
    { value: 10, label: 'Octubre' },
    { value: 11, label: 'Noviembre' },
    { value: 12, label: 'Diciembre' }
  ];

  years: number[] = [];
  categories: Category[] = [];
  products: Product[] = [];

  readonly displayedSalesColumns = [
    'numeroVenta',
    'fechaVenta',
    'usuarioNombre',
    'cantidadItems',
    'subtotal',
    'totalImpuestos',
    'totalDescuentos',
    'total'
  ];

  readonly displayedProductColumns = [
    'productoNombre',
    'categoriaNombre',
    'cantidadVendida',
    'totalVendido'
  ];

  private readonly destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private reportsService: ReportsService,
    private categoryService: CategoryService,
    private productService: ProductService,
    private snackBar: MatSnackBar
  ) {
    const today = new Date();
    this.filterForm = this.fb.group({
      filterType: ['month', Validators.required],
      month: [today.getMonth() + 1, Validators.required],
      year: [today.getFullYear(), Validators.required],
      startDate: [null],
      endDate: [null],
      productId: [null],
      categoryId: [null]
    });

    this.buildYearOptions(today.getFullYear());
  }

  ngOnInit(): void {
    this.filterForm.get('filterType')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.updateDateValidators();
      });

    this.updateDateValidators();
    this.loadCategories();
    this.loadProducts();
    this.loadReport();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get hasData(): boolean {
    return !!this.report && (this.report.ventas?.length > 0 || this.report.productos?.length > 0);
  }

  get selectedProductName(): string | null {
    const productId = this.filterForm.get('productId')?.value;
    if (!productId) {
      return null;
    }
    const product = this.products.find(p => p.id === productId);
    return product ? product.nombre : null;
  }

  get selectedCategoryName(): string | null {
    const categoryId = this.filterForm.get('categoryId')?.value;
    if (!categoryId) {
      return null;
    }
    const category = this.categories.find(c => c.id === categoryId);
    return category ? category.nombre : null;
  }

  applyFilters(): void {
    if (this.filterForm.invalid) {
      this.filterForm.markAllAsTouched();
      return;
    }

    if (!this.validateDateRange()) {
      return;
    }

    this.loadReport();
  }

  clearFilters(): void {
    const today = new Date();
    this.filterForm.reset({
      filterType: 'month',
      month: today.getMonth() + 1,
      year: today.getFullYear(),
      startDate: null,
      endDate: null,
      productId: null,
      categoryId: null
    });
    this.updateDateValidators();
    this.loadReport();
  }

  trackById(_: number, item: { id: number }): number {
    return item.id;
  }

  private loadReport(): void {
    const filterType = this.filterForm.get('filterType')?.value ?? 'month';
    const filter = this.buildFilterPayload(filterType);

    this.loading = true;
    this.errorMessage = '';

    this.reportsService.getSalesReport(filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (report) => {
          this.report = report;
          this.currentFilterDescription = report.periodoDescripcion;
          this.loading = false;
        },
        error: (error) => {
          console.log(error);
          console.error('Error loading sales report', error);
          this.loading = false;
          this.report = null;
          this.errorMessage = 'No se pudo cargar el reporte de ventas. Inténtalo nuevamente.';
          this.snackBar.open(this.errorMessage, 'Cerrar', { duration: 4000 });
        }
      });
  }

  private loadCategories(): void {
    this.categoryService.getActiveCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categories) => {
          this.categories = categories;
        },
        error: (error) => {
          console.error('Error loading categories', error);
        }
      });
  }

  private loadProducts(): void {
    this.productService.getProducts({ pageNumber: 1, pageSize: 500, orderBy: 'nombre', orderDescending: false })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.products = result.items;
        },
        error: (error) => {
          console.error('Error loading products', error);
        }
      });
  }

  private buildFilterPayload(filterType: string): SalesReportFilter {
    const filter: SalesReportFilter = {};
    const { month, year, startDate, endDate, productId, categoryId } = this.filterForm.value;

    if (filterType === 'month') {
      if (month != null) {
        filter.month = month;
      }
      if (year != null) {
        filter.year = year;
      }
    } else if (filterType === 'year') {
      if (year != null) {
        filter.year = year;
      }
    } else if (filterType === 'range') {
      if (startDate) {
        filter.startDate = this.formatDateForApi(startDate);
      }
      if (endDate) {
        filter.endDate = this.formatDateForApi(endDate);
      }
    }

    if (productId != null) {
      filter.productId = productId;
    }

    if (categoryId != null) {
      filter.categoryId = categoryId;
    }

    return filter;
  }

  private validateDateRange(): boolean {
    const filterType = this.filterForm.get('filterType')?.value;
    if (filterType !== 'range') {
      return true;
    }

    const startDate: Date | null = this.filterForm.get('startDate')?.value;
    const endDate: Date | null = this.filterForm.get('endDate')?.value;

    if (startDate && endDate && endDate < startDate) {
      this.snackBar.open('La fecha final no puede ser menor que la fecha inicial.', 'Cerrar', { duration: 4000 });
      return false;
    }

    return true;
  }

  private formatDateForApi(date: Date | string): string {
    const parsedDate = typeof date === 'string' ? new Date(date) : date;
    const year = parsedDate.getFullYear();
    const month = `${parsedDate.getMonth() + 1}`.padStart(2, '0');
    const day = `${parsedDate.getDate()}`.padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private updateDateValidators(): void {
    const filterType = this.filterForm.get('filterType')?.value;
    const monthControl = this.filterForm.get('month');
    const yearControl = this.filterForm.get('year');
    const startControl = this.filterForm.get('startDate');
    const endControl = this.filterForm.get('endDate');

    monthControl?.clearValidators();
    yearControl?.clearValidators();
    startControl?.clearValidators();
    endControl?.clearValidators();

    if (filterType === 'month') {
      monthControl?.setValidators([Validators.required]);
      yearControl?.setValidators([Validators.required]);
    } else if (filterType === 'year') {
      yearControl?.setValidators([Validators.required]);
    } else if (filterType === 'range') {
      startControl?.setValidators([Validators.required]);
    }

    monthControl?.updateValueAndValidity({ emitEvent: false });
    yearControl?.updateValueAndValidity({ emitEvent: false });
    startControl?.updateValueAndValidity({ emitEvent: false });
    endControl?.updateValueAndValidity({ emitEvent: false });
  }

  private buildYearOptions(currentYear: number): void {
    const years: number[] = [];
    for (let i = 0; i < 6; i++) {
      years.push(currentYear - i);
    }
    this.years = years;
  }
}
