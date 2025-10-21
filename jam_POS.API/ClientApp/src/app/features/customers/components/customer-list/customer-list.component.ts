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
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { CustomerService } from '../../services/customer.service';
import { Customer, CreateCustomerDto, UpdateCustomerDto, CustomerFilter } from '../../models/customer.model';
import { PagedResult } from '../../../../core/models/pagination.model';

@Component({
  selector: 'app-customer-list',
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
    MatSortModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatChipsModule,
    MatDividerModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent implements OnInit {
  customers: Customer[] = [];
  pagedResult: PagedResult<Customer> | null = null;
  customerForm: FormGroup;
  filterForm: FormGroup;

  isEditing = false;
  editingId: number | null = null;
  showForm = false;
  loading = false;

  displayedColumns: string[] = ['nombre', 'contacto', 'documento', 'fechaNacimiento', 'activo', 'acciones'];

  pageSize = 10;
  pageNumber = 1;
  pageSizeOptions = [5, 10, 20, 50];
  sortBy = 'Nombre';
  sortDescending = false;

  constructor(
    private customerService: CustomerService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.customerForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      apellido: ['', [Validators.maxLength(100)]],
      email: ['', [Validators.email, Validators.maxLength(150)]],
      telefono: ['', [Validators.maxLength(50)]],
      documento: ['', [Validators.maxLength(50)]],
      direccion: ['', [Validators.maxLength(300)]],
      notas: ['', [Validators.maxLength(500)]],
      fechaNacimiento: [null],
      activo: [true]
    });

    this.filterForm = this.fb.group({
      searchTerm: [''],
      activo: [null]
    });
  }

  ngOnInit(): void {
    this.loadCustomers();

    this.filterForm.valueChanges.subscribe(() => {
      this.pageNumber = 1;
      this.loadCustomers();
    });
  }

  loadCustomers(): void {
    this.loading = true;

    const filter: CustomerFilter = {
      ...this.filterForm.value,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      orderBy: this.sortBy,
      orderDescending: this.sortDescending
    };

    this.customerService.getCustomers(filter).subscribe({
      next: (result) => {
        this.pagedResult = result;
        this.customers = result.items;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar clientes:', error);
        this.snackBar.open('Error al cargar los clientes', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadCustomers();
  }

  onSortChange(sort: Sort): void {
    this.sortBy = this.mapSortField(sort.active);
    this.sortDescending = sort.direction === 'desc';
    this.loadCustomers();
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

  editCustomer(customer: Customer): void {
    this.isEditing = true;
    this.editingId = customer.id;
    this.showForm = true;

    this.customerForm.patchValue({
      nombre: customer.nombre,
      apellido: customer.apellido || '',
      email: customer.email || '',
      telefono: customer.telefono || '',
      documento: customer.documento || '',
      direccion: customer.direccion || '',
      notas: customer.notas || '',
      fechaNacimiento: customer.fechaNacimiento ? new Date(customer.fechaNacimiento) : null,
      activo: customer.activo
    });
  }

  cancelEdit(): void {
    this.resetForm();
    this.showForm = false;
  }

  onSubmit(): void {
    if (this.customerForm.invalid) {
      this.snackBar.open('Por favor complete los campos requeridos', 'Cerrar', { duration: 3000 });
      return;
    }

    const formValue = this.customerForm.value;
    const payload: CreateCustomerDto = {
      nombre: formValue.nombre,
      apellido: formValue.apellido || null,
      email: formValue.email || null,
      telefono: formValue.telefono || null,
      documento: formValue.documento || null,
      direccion: formValue.direccion || null,
      notas: formValue.notas || null,
      fechaNacimiento: formValue.fechaNacimiento ? new Date(formValue.fechaNacimiento).toISOString() : null,
      activo: formValue.activo ?? true
    };

    if (this.isEditing && this.editingId) {
      const updatePayload: UpdateCustomerDto = { ...payload, id: this.editingId };
      this.updateCustomer(updatePayload);
    } else {
      this.createCustomer(payload);
    }
  }

  deleteCustomer(id: number): void {
    const confirmed = confirm('¿Está seguro que desea eliminar este cliente?');
    if (!confirmed) {
      return;
    }

    this.customerService.deleteCustomer(id).subscribe({
      next: () => {
        this.snackBar.open('Cliente eliminado correctamente', 'Cerrar', { duration: 3000 });
        this.loadCustomers();
      },
      error: (error) => {
        console.error('Error al eliminar cliente:', error);
        const errorMsg = error.error?.message || 'Error al eliminar el cliente';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
      }
    });
  }

  getActiveFilterCount(): number {
    const filters = this.filterForm.value;
    let count = 0;

    if (filters.searchTerm) count++;
    if (filters.activo !== null && filters.activo !== undefined) count++;

    return count;
  }

  getStatusChipColor(isActive: boolean): string {
    return isActive ? 'primary' : 'warn';
  }

  private createCustomer(payload: CreateCustomerDto): void {
    this.loading = true;
    this.customerService.createCustomer(payload).subscribe({
      next: () => {
        this.snackBar.open('Cliente creado correctamente', 'Cerrar', { duration: 3000 });
        this.loadCustomers();
        this.resetForm();
        this.showForm = false;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al crear cliente:', error);
        const errorMsg = error.error?.message || 'Error al crear el cliente';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private updateCustomer(payload: UpdateCustomerDto): void {
    this.loading = true;
    this.customerService.updateCustomer(payload.id, payload).subscribe({
      next: () => {
        this.snackBar.open('Cliente actualizado correctamente', 'Cerrar', { duration: 3000 });
        this.loadCustomers();
        this.resetForm();
        this.showForm = false;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al actualizar cliente:', error);
        const errorMsg = error.error?.message || 'Error al actualizar el cliente';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private resetForm(): void {
    this.customerForm.reset({
      nombre: '',
      apellido: '',
      email: '',
      telefono: '',
      documento: '',
      direccion: '',
      notas: '',
      fechaNacimiento: null,
      activo: true
    });
    this.isEditing = false;
    this.editingId = null;
  }

  private mapSortField(active: string | undefined): string {
    switch (active) {
      case 'email':
        return 'Email';
      case 'telefono':
        return 'Telefono';
      case 'documento':
        return 'Documento';
      case 'fechaNacimiento':
        return 'FechaNacimiento';
      case 'activo':
        return 'Activo';
      case 'createdAt':
        return 'CreatedAt';
      default:
        return 'Nombre';
    }
  }
}
