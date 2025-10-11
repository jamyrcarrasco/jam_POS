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
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { TaxService } from '../../services/tax.service';
import { Tax } from '../../models/tax.model';

@Component({
  selector: 'app-tax-list',
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
    MatSelectModule,
    MatSlideToggleModule,
    MatChipsModule,
    MatDividerModule
  ],
  templateUrl: './tax-list.component.html',
  styleUrls: ['./tax-list.component.scss']
})
export class TaxListComponent implements OnInit {
  taxes: Tax[] = [];
  taxForm: FormGroup;
  
  isEditing = false;
  editingId: number | null = null;
  showForm = false;
  loading = false;
  displayedColumns: string[] = ['nombre', 'tipo', 'valor', 'defecto', 'activo', 'acciones'];

  tiposImpuesto = [
    { value: 'PORCENTUAL', label: 'Porcentual (%)' },
    { value: 'FIJO', label: 'Monto Fijo ($)' }
  ];

  constructor(
    private taxService: TaxService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.taxForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.maxLength(500)]],
      tipo: ['PORCENTUAL', [Validators.required]],
      porcentaje: [0, [Validators.min(0), Validators.max(100)]],
      montoFijo: [0, [Validators.min(0)]],
      aplicaPorDefecto: [false],
      activo: [true]
    });

    // Watch tipo changes to enable/disable fields
    this.taxForm.get('tipo')?.valueChanges.subscribe(tipo => {
      if (tipo === 'PORCENTUAL') {
        this.taxForm.get('porcentaje')?.enable();
        this.taxForm.get('montoFijo')?.disable();
        this.taxForm.patchValue({ montoFijo: null });
      } else {
        this.taxForm.get('porcentaje')?.disable();
        this.taxForm.get('montoFijo')?.enable();
        this.taxForm.patchValue({ porcentaje: 0 });
      }
    });
  }

  ngOnInit(): void {
    this.loadTaxes();
  }

  loadTaxes(): void {
    this.loading = true;
    this.taxService.getTaxes().subscribe({
      next: (taxes) => {
        this.taxes = taxes;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar impuestos:', error);
        this.snackBar.open('Error al cargar los impuestos', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.resetForm();
    }
  }

  onSubmit(): void {
    if (this.taxForm.valid) {
      const taxData = this.taxForm.getRawValue(); // getRawValue para incluir campos disabled
      
      if (this.isEditing && this.editingId) {
        this.updateTax(taxData);
      } else {
        this.createTax(taxData);
      }
    } else {
      this.snackBar.open('Por favor complete todos los campos requeridos', 'Cerrar', { duration: 3000 });
    }
  }

  private createTax(taxData: any): void {
    this.loading = true;
    this.taxService.createTax(taxData).subscribe({
      next: () => {
        this.loadTaxes();
        this.resetForm();
        this.showForm = false;
        this.snackBar.open('Impuesto creado exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al crear el impuesto';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private updateTax(taxData: any): void {
    if (!this.editingId) return;
    
    this.loading = true;
    this.taxService.updateTax(this.editingId, taxData).subscribe({
      next: () => {
        this.loadTaxes();
        this.resetForm();
        this.showForm = false;
        this.snackBar.open('Impuesto actualizado exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al actualizar el impuesto';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  editTax(tax: Tax): void {
    this.isEditing = true;
    this.editingId = tax.id;
    this.showForm = true;
    this.taxForm.patchValue({
      nombre: tax.nombre,
      descripcion: tax.descripcion,
      tipo: tax.tipo,
      porcentaje: tax.porcentaje,
      montoFijo: tax.montoFijo,
      aplicaPorDefecto: tax.aplicaPorDefecto,
      activo: tax.activo
    });

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  deleteTax(id: number, nombre: string): void {
    if (confirm(`¿Está seguro de que desea eliminar el impuesto "${nombre}"?`)) {
      this.loading = true;
      this.taxService.deleteTax(id).subscribe({
        next: () => {
          this.loadTaxes();
          this.snackBar.open('Impuesto eliminado exitosamente', 'Cerrar', { duration: 3000 });
          this.loading = false;
        },
        error: (error) => {
          const errorMsg = error.error?.message || 'Error al eliminar el impuesto';
          this.snackBar.open(errorMsg, 'Cerrar', { duration: 5000 });
          this.loading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.taxForm.reset({ 
      activo: true,
      tipo: 'PORCENTUAL',
      aplicaPorDefecto: false,
      porcentaje: 0,
      montoFijo: 0
    });
    this.isEditing = false;
    this.editingId = null;
  }

  cancelEdit(): void {
    this.resetForm();
    this.showForm = false;
  }

  getValorDisplay(tax: Tax): string {
    if (tax.tipo === 'PORCENTUAL') {
      return `${tax.porcentaje}%`;
    } else {
      return `$${tax.montoFijo?.toFixed(2) || '0.00'}`;
    }
  }
}

