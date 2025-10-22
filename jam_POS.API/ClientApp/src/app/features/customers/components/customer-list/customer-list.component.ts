import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
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
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog } from '@angular/material/dialog';
import { CustomerService } from '../../services/customer.service';
import { Customer, CustomerFilter } from '../../models/customer.model';
import { PagedResult } from '../../../../core/models/pagination.model';
import { CustomerModalComponent, CustomerModalData } from '../customer-modal/customer-modal.component';

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
    MatChipsModule,
    MatDividerModule
  ],
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent implements OnInit {
  customers: Customer[] = [];
  pagedResult: PagedResult<Customer> | null = null;
  filterForm: FormGroup;
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
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
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

  openCustomerModal(customer?: Customer): void {
    const dialogData: CustomerModalData = {
      customer: customer,
      isEdit: !!customer
    };

    const dialogRef = this.dialog.open(CustomerModalComponent, {
      data: dialogData,
      width: '600px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.success) {
        this.loadCustomers();
      }
    });
  }

  editCustomer(customer: Customer): void {
    this.openCustomerModal(customer);
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
