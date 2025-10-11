import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { MatTabsModule } from '@angular/material/tabs';
import { POSConfigService } from '../../services/pos-config.service';
import { TaxService } from '../../../taxes/services/tax.service';
import { POSConfig } from '../../models/pos-config.model';
import { Tax } from '../../../taxes/models/tax.model';

@Component({
  selector: 'app-pos-config',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatTooltipModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatDividerModule,
    MatTabsModule
  ],
  templateUrl: './pos-config.component.html',
  styleUrls: ['./pos-config.component.scss']
})
export class POSConfigComponent implements OnInit {
  configForm: FormGroup;
  currentConfig: POSConfig | null = null;
  taxes: Tax[] = [];
  loading = false;
  isEditing = false;

  formatosPapel = [
    { value: 'TERMICO_58', label: 'Térmico 58mm' },
    { value: 'TERMICO_80', label: 'Térmico 80mm' },
    { value: 'A4', label: 'A4' },
    { value: 'LETTER', label: 'Carta' }
  ];

  monedas = [
    { value: 'DOP', label: 'Peso Dominicano (DOP)' },
    { value: 'USD', label: 'Dólar Americano (USD)' },
    { value: 'EUR', label: 'Euro (EUR)' },
    { value: 'MXN', label: 'Peso Mexicano (MXN)' }
  ];

  simbolosMoneda = [
    { value: '$', label: '$ (Peso/Dólar)' },
    { value: '€', label: '€ (Euro)' },
    { value: 'RD$', label: 'RD$ (Peso Dominicano)' },
    { value: 'USD', label: 'USD (Dólar)' }
  ];

  constructor(
    private posConfigService: POSConfigService,
    private taxService: TaxService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.configForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadConfig();
    this.loadTaxes();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      // Configuración de Recibos/Facturas
      prefijoRecibo: ['REC', [Validators.required, Validators.maxLength(10)]],
      prefijoFactura: ['FAC', [Validators.required, Validators.maxLength(10)]],
      siguienteNumeroRecibo: [1, [Validators.required, Validators.min(1)]],
      siguienteNumeroFactura: [1, [Validators.required, Validators.min(1)]],
      mensajePieRecibo: ['', [Validators.maxLength(500)]],
      incluirLogoRecibo: [true],

      // Comportamiento de Ventas
      impuestoPorDefectoId: [null],
      permitirDescuentos: [true],
      permitirDevoluciones: [true],
      tiempoLimiteAnulacionMinutos: [30, [Validators.min(0), Validators.max(1440)]],
      descuentoMaximoPorcentaje: [100, [Validators.min(0), Validators.max(100)]],

      // Configuración de Moneda
      monedaPorDefecto: ['DOP', [Validators.required, Validators.maxLength(3)]],
      simboloMoneda: ['$', [Validators.required, Validators.maxLength(10)]],
      decimalesMoneda: [2, [Validators.min(0), Validators.max(4)]],

      // Configuración de Impresión
      formatoPapel: ['TERMICO_58', [Validators.required, Validators.maxLength(50)]],
      imprimirAutomaticamente: [false],
      imprimirCopiaCliente: [true],

      // Configuración de Métodos de Pago
      efectivoHabilitado: [true],
      tarjetaHabilitado: [true],
      transferenciaHabilitado: [false],
      creditoHabilitado: [false],

      // Configuración de Modo Operación
      modoOfflineHabilitado: [false],
      intervaloSincronizacionMinutos: [15, [Validators.min(1), Validators.max(1440)]]
    });
  }

  loadConfig(): void {
    this.loading = true;
    this.posConfigService.getConfig().subscribe({
      next: (config) => {
        this.currentConfig = config;
        this.isEditing = true;
        this.configForm.patchValue(config);
        this.loading = false;
      },
      error: (error) => {
        if (error.status === 404) {
          // No hay configuración, se creará una nueva
          this.currentConfig = null;
          this.isEditing = false;
          this.loading = false;
        } else {
          console.error('Error al cargar configuración:', error);
          this.snackBar.open('Error al cargar la configuración POS', 'Cerrar', { duration: 3000 });
          this.loading = false;
        }
      }
    });
  }

  loadTaxes(): void {
    this.taxService.getActiveTaxes().subscribe({
      next: (taxes) => {
        this.taxes = taxes;
      },
      error: (error) => {
        console.error('Error al cargar impuestos:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.configForm.valid) {
      this.loading = true;
      const configData = this.configForm.value;

      if (this.isEditing && this.currentConfig) {
        this.updateConfig(configData);
      } else {
        this.createConfig(configData);
      }
    } else {
      this.snackBar.open('Por favor complete todos los campos requeridos', 'Cerrar', { duration: 3000 });
    }
  }

  private createConfig(configData: any): void {
    this.posConfigService.createConfig(configData).subscribe({
      next: (config) => {
        this.currentConfig = config;
        this.isEditing = true;
        this.snackBar.open('Configuración POS creada exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al crear la configuración POS';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private updateConfig(configData: any): void {
    if (!this.currentConfig) return;

    this.posConfigService.updateConfig(this.currentConfig.id, configData).subscribe({
      next: (config) => {
        this.currentConfig = config;
        this.snackBar.open('Configuración POS actualizada exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al actualizar la configuración POS';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  resetForm(): void {
    if (this.currentConfig) {
      this.configForm.patchValue(this.currentConfig);
    } else {
      this.configForm.reset({
        prefijoRecibo: 'REC',
        prefijoFactura: 'FAC',
        siguienteNumeroRecibo: 1,
        siguienteNumeroFactura: 1,
        incluirLogoRecibo: true,
        permitirDescuentos: true,
        permitirDevoluciones: true,
        tiempoLimiteAnulacionMinutos: 30,
        descuentoMaximoPorcentaje: 100,
        monedaPorDefecto: 'DOP',
        simboloMoneda: '$',
        decimalesMoneda: 2,
        formatoPapel: 'TERMICO_58',
        imprimirAutomaticamente: false,
        imprimirCopiaCliente: true,
        efectivoHabilitado: true,
        tarjetaHabilitado: true,
        transferenciaHabilitado: false,
        creditoHabilitado: false,
        modoOfflineHabilitado: false,
        intervaloSincronizacionMinutos: 15
      });
    }
  }

  previewNumeroRecibo(): void {
    this.posConfigService.getSiguienteNumeroRecibo().subscribe({
      next: (numero) => {
        this.snackBar.open(`Próximo número de recibo: ${numero}`, 'Cerrar', { duration: 3000 });
      },
      error: (error) => {
        console.error('Error al obtener número de recibo:', error);
      }
    });
  }

  previewNumeroFactura(): void {
    this.posConfigService.getSiguienteNumeroFactura().subscribe({
      next: (numero) => {
        this.snackBar.open(`Próximo número de factura: ${numero}`, 'Cerrar', { duration: 3000 });
      },
      error: (error) => {
        console.error('Error al obtener número de factura:', error);
      }
    });
  }
}
