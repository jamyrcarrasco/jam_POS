import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { SaleService } from '../../services/sale.service';
import { ProductService } from '../../../products/services/product.service';
import { TaxService } from '../../../taxes/services/tax.service';
import { POSConfigService } from '../../../pos-config/services/pos-config.service';
import { Sale, SaleItem, Payment, CreateSaleRequest } from '../../models/sale.model';
import { Product } from '../../../products/models/product.model';
import { Tax } from '../../../taxes/models/tax.model';
import { POSConfig } from '../../../pos-config/models/pos-config.model';

@Component({
  selector: 'app-sales-pos',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatDividerModule,
    MatChipsModule,
    MatTooltipModule,
    MatDialogModule
  ],
  templateUrl: './sales-pos.component.html',
  styleUrls: ['./sales-pos.component.scss']
})
export class SalesPOSComponent implements OnInit {
  // Formulario para búsqueda de productos
  productSearchForm: FormGroup;
  
  // Carrito de compras
  cartItems: SaleItem[] = [];
  
  // Productos disponibles
  products: Product[] = [];
  filteredProducts: Product[] = [];
  
  // Configuración POS
  posConfig: POSConfig | null = null;
  
  // Estados
  loading = false;
  processing = false;
  
  // Totales del carrito
  cartSubtotal = 0;
  cartTaxes = 0;
  cartDiscounts = 0;
  cartTotal = 0;
  
  // Métodos de pago disponibles
  availablePaymentMethods: string[] = [];
  
  // Pagos de la venta
  payments: Payment[] = [];
  remainingAmount = 0;

  constructor(
    private fb: FormBuilder,
    private saleService: SaleService,
    private productService: ProductService,
    private taxService: TaxService,
    private posConfigService: POSConfigService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    this.productSearchForm = this.fb.group({
      searchTerm: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadPOSConfig();
    this.setupPaymentMethods();
  }

  private loadProducts(): void {
    this.loading = true;
    this.productService.getProducts().subscribe({
      next: (result) => {
        this.products = result.items;
        this.filteredProducts = result.items;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error cargando productos:', error);
        this.snackBar.open('Error al cargar productos', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private loadPOSConfig(): void {
    this.posConfigService.getConfig().subscribe({
      next: (config) => {
        this.posConfig = config;
      },
      error: (error) => {
        console.error('Error cargando configuración POS:', error);
        // Usar configuración por defecto
        this.posConfig = {
          efectivoHabilitado: true,
          tarjetaHabilitado: true,
          transferenciaHabilitado: false,
          creditoHabilitado: false,
          simboloMoneda: '$',
          monedaPorDefecto: 'DOP'
        } as POSConfig;
      }
    });
  }

  private setupPaymentMethods(): void {
    this.availablePaymentMethods = ['EFECTIVO', 'TARJETA'];
  }

  onSearchProducts(): void {
    const searchTerm = this.productSearchForm.get('searchTerm')?.value?.toLowerCase();
    
    if (!searchTerm) {
      this.filteredProducts = this.products;
      return;
    }

    this.filteredProducts = this.products.filter(product =>
      product.nombre.toLowerCase().includes(searchTerm) ||
      product.codigoBarras?.toLowerCase().includes(searchTerm) ||
      product.descripcion?.toLowerCase().includes(searchTerm)
    );
  }

  addProductToCart(product: Product): void {
    const existingItem = this.cartItems.find(item => item.productoId === product.id);
    
    if (existingItem) {
      existingItem.cantidad += 1;
      this.recalculateItemTotal(existingItem);
    } else {
      const newItem: SaleItem = {
        id: 0,
        ventaId: 0,
        productoId: product.id,
        productoNombre: product.nombre,
        productoCodigo: product.codigoBarras,
        cantidad: 1,
        precioUnitario: product.precio,
        subtotal: product.precio,
        descuentoPorcentaje: 0,
        descuentoMonto: 0,
        totalImpuestos: 0,
        total: product.precio,
        notas: '',
        createdAt: new Date(),
        updatedAt: new Date()
      };
      
      this.recalculateItemTotal(newItem);
      this.cartItems.push(newItem);
    }
    
    this.recalculateCartTotals();
    this.clearSearch();
  }

  removeItemFromCart(index: number): void {
    this.cartItems.splice(index, 1);
    this.recalculateCartTotals();
  }

  updateItemQuantity(item: SaleItem, quantity: number): void {
    if (quantity <= 0) {
      this.removeItemFromCart(this.cartItems.indexOf(item));
      return;
    }
    
    item.cantidad = quantity;
    this.recalculateItemTotal(item);
    this.recalculateCartTotals();
  }

  updateItemPrice(item: SaleItem, price: number): void {
    if (price < 0) return;
    
    item.precioUnitario = price;
    this.recalculateItemTotal(item);
    this.recalculateCartTotals();
  }

  applyItemDiscount(item: SaleItem, discount: number): void {
    if (discount < 0 || discount > 100) return;
    
    item.descuentoPorcentaje = discount;
    this.recalculateItemTotal(item);
    this.recalculateCartTotals();
  }

  private recalculateItemTotal(item: SaleItem): void {
    // Calcular subtotal
    item.subtotal = item.cantidad * item.precioUnitario;
    
    // Aplicar descuento
    const discountAmount = item.subtotal * (item.descuentoPorcentaje / 100);
    item.descuentoMonto = discountAmount;
    
    // Calcular impuestos (simplificado - usar configuración POS)
    const subtotalAfterDiscount = item.subtotal - item.descuentoMonto;
    item.totalImpuestos = this.calculateTaxes(subtotalAfterDiscount);
    
    // Calcular total
    item.total = subtotalAfterDiscount + item.totalImpuestos;
  }

  private recalculateCartTotals(): void {
    this.cartSubtotal = this.cartItems.reduce((sum, item) => sum + item.subtotal, 0);
    this.cartDiscounts = this.cartItems.reduce((sum, item) => sum + item.descuentoMonto, 0);
    this.cartTaxes = this.cartItems.reduce((sum, item) => sum + item.totalImpuestos, 0);
    this.cartTotal = this.cartSubtotal - this.cartDiscounts + this.cartTaxes;
    
    this.remainingAmount = this.cartTotal - this.payments.reduce((sum, payment) => sum + payment.monto, 0);
  }

  private calculateTaxes(amount: number): number {
    // Simplificado - en un caso real, usarías la configuración POS para obtener el impuesto por defecto
    if (this.posConfig?.impuestoPorDefectoId) {
      // Aquí deberías obtener el impuesto específico y calcularlo
      return amount * 0.18; // 18% por defecto
    }
    return 0;
  }

  clearSearch(): void {
    this.productSearchForm.reset();
    this.filteredProducts = this.products;
  }

  clearCart(): void {
    this.cartItems = [];
    this.payments = [];
    this.recalculateCartTotals();
  }

  addPayment(method: string): void {
    if (this.remainingAmount <= 0) {
      this.snackBar.open('La venta ya está pagada completamente', 'Cerrar', { duration: 3000 });
      return;
    }

    const amount = this.remainingAmount;
    const newPayment: Payment = {
      id: 0,
      ventaId: 0,
      metodoPago: method,
      monto: amount,
      fechaPago: new Date(),
      createdAt: new Date()
    };

    this.payments.push(newPayment);
    this.recalculateCartTotals();
  }

  removePayment(index: number): void {
    this.payments.splice(index, 1);
    this.recalculateCartTotals();
  }

  processSale(): void {
    if (this.cartItems.length === 0) {
      this.snackBar.open('El carrito está vacío', 'Cerrar', { duration: 3000 });
      return;
    }

    if (this.remainingAmount > 0.01) {
      this.snackBar.open('La venta no está completamente pagada', 'Cerrar', { duration: 3000 });
      return;
    }

    this.processing = true;

    const saleRequest: CreateSaleRequest = {
      items: this.cartItems.map(item => ({
        productoId: item.productoId,
        cantidad: item.cantidad,
        precioUnitario: item.precioUnitario,
        descuentoPorcentaje: item.descuentoPorcentaje,
        descuentoMonto: item.descuentoMonto,
        notas: item.notas
      })),
      pagos: this.payments.map(payment => ({
        metodoPago: payment.metodoPago,
        monto: payment.monto,
        referencia: payment.referencia,
        banco: payment.banco,
        tipoTarjeta: payment.tipoTarjeta,
        notas: payment.notas
      }))
    };

    this.saleService.createSale(saleRequest).subscribe({
      next: (sale) => {
        this.snackBar.open(`Venta procesada exitosamente: ${sale.numeroVenta}`, 'Cerrar', { duration: 5000 });
        this.clearCart();
        this.processing = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al procesar la venta';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 5000 });
        this.processing = false;
      }
    });
  }

  formatCurrency(amount: number): string {
    const symbol = this.posConfig?.simboloMoneda || '$';
    return `${symbol}${amount.toFixed(2)}`;
  }

  getPaymentIcon(method: string): string {
    switch (method) {
      case 'EFECTIVO': return 'money';
      case 'TARJETA': return 'credit_card';
      case 'TRANSFERENCIA': return 'account_balance';
      case 'CREDITO': return 'account_balance_wallet';
      default: return 'payment';
    }
  }

  trackByProductId(index: number, product: Product): number {
    return product.id;
  }

  trackByItemId(index: number, item: SaleItem): number {
    return item.id || index;
  }

  onPriceChange(item: SaleItem, event: Event): void {
    const input = event.target as HTMLInputElement;
    const newPrice = parseFloat(input.value);
    if (!isNaN(newPrice) && newPrice >= 0) {
      this.updateItemPrice(item, newPrice);
    }
  }

  onDiscountChange(item: SaleItem, event: Event): void {
    const input = event.target as HTMLInputElement;
    const newDiscount = parseFloat(input.value);
    if (!isNaN(newDiscount) && newDiscount >= 0 && newDiscount <= 100) {
      this.applyItemDiscount(item, newDiscount);
    }
  }
}
