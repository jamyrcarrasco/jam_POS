import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { CustomerService } from '../../services/customer.service';
import { Customer, CreateCustomerDto, UpdateCustomerDto } from '../../models/customer.model';
import { Subject, takeUntil } from 'rxjs';

export interface CustomerModalData {
  customer?: Customer;
  isEdit: boolean;
}

@Component({
  selector: 'app-customer-modal',
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
    MatSlideToggleModule,
    MatDividerModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './customer-modal.component.html',
  styleUrls: ['./customer-modal.component.scss']
})
export class CustomerModalComponent implements OnInit, OnDestroy {
  customerForm: FormGroup;
  loading = false;
  private destroy$ = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<CustomerModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CustomerModalData,
    private fb: FormBuilder,
    private customerService: CustomerService,
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
  }

  ngOnInit(): void {
    if (this.data.isEdit && this.data.customer) {
      this.loadCustomerData();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCustomerData(): void {
    if (this.data.customer) {
      this.customerForm.patchValue({
        nombre: this.data.customer.nombre,
        apellido: this.data.customer.apellido || '',
        email: this.data.customer.email || '',
        telefono: this.data.customer.telefono || '',
        documento: this.data.customer.documento || '',
        direccion: this.data.customer.direccion || '',
        notas: this.data.customer.notas || '',
        fechaNacimiento: this.data.customer.fechaNacimiento ? new Date(this.data.customer.fechaNacimiento) : null,
        activo: this.data.customer.activo
      });
    }
  }

  onSubmit(): void {
    if (this.customerForm.valid) {
      this.loading = true;
      const formData = this.customerForm.value;

      if (this.data.isEdit && this.data.customer) {
        this.updateCustomer(formData);
      } else {
        this.createCustomer(formData);
      }
    }
  }

  private createCustomer(formData: any): void {
    const payload: CreateCustomerDto = {
      nombre: formData.nombre,
      apellido: formData.apellido || null,
      email: formData.email || null,
      telefono: formData.telefono || null,
      documento: formData.documento || null,
      direccion: formData.direccion || null,
      notas: formData.notas || null,
      fechaNacimiento: formData.fechaNacimiento ? new Date(formData.fechaNacimiento).toISOString() : null,
      activo: formData.activo ?? true
    };

    this.customerService.createCustomer(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.loading = false;
          this.snackBar.open('Cliente creado exitosamente', 'Cerrar', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.dialogRef.close({ success: true, customer: response });
        },
        error: (error) => {
          this.loading = false;
          console.error('Error creating customer:', error);
          const errorMsg = error.error?.message || 'Error al crear el cliente';
          this.snackBar.open(errorMsg, 'Cerrar', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }

  private updateCustomer(formData: any): void {
    if (!this.data.customer?.id) return;

    const payload: UpdateCustomerDto = {
      id: this.data.customer.id,
      nombre: formData.nombre,
      apellido: formData.apellido || null,
      email: formData.email || null,
      telefono: formData.telefono || null,
      documento: formData.documento || null,
      direccion: formData.direccion || null,
      notas: formData.notas || null,
      fechaNacimiento: formData.fechaNacimiento ? new Date(formData.fechaNacimiento).toISOString() : null,
      activo: formData.activo ?? true
    };

    this.customerService.updateCustomer(this.data.customer.id, payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.loading = false;
          this.snackBar.open('Cliente actualizado exitosamente', 'Cerrar', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.dialogRef.close({ success: true, customer: response });
        },
        error: (error) => {
          this.loading = false;
          console.error('Error updating customer:', error);
          const errorMsg = error.error?.message || 'Error al actualizar el cliente';
          this.snackBar.open(errorMsg, 'Cerrar', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }

  onCancel(): void {
    this.dialogRef.close({ success: false });
  }
}
