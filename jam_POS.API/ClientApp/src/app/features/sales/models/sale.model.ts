import { BaseEntity } from '../../../core/models/base.model';

export interface Sale extends BaseEntity {
  numeroVenta: string;
  fechaVenta: Date;
  notas?: string;
  subtotal: number;
  totalImpuestos: number;
  totalDescuentos: number;
  total: number;
  estado: string;
  fechaCancelacion?: Date;
  motivoCancelacion?: string;
  clienteId?: number;
  clienteNombre?: string;
  usuarioId: number;
  usuarioNombre: string;
  items: SaleItem[];
  pagos: Payment[];
}

export interface SaleItem extends BaseEntity {
  ventaId: number;
  productoId: number;
  productoNombre: string;
  productoCodigo?: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
  descuentoPorcentaje: number;
  descuentoMonto: number;
  totalImpuestos: number;
  total: number;
  notas?: string;
}

export interface Payment {
  id: number;
  ventaId: number;
  metodoPago: string;
  monto: number;
  referencia?: string;
  banco?: string;
  tipoTarjeta?: string;
  fechaPago: Date;
  notas?: string;
  createdAt: Date;
}

export interface SaleSummary {
  id: number;
  numeroVenta: string;
  fechaVenta: Date;
  total: number;
  estado: string;
  usuarioNombre: string;
  cantidadItems: number;
}

export interface CreateSaleRequest {
  notas?: string;
  clienteId?: number;
  items: CreateSaleItemRequest[];
  pagos: CreatePaymentRequest[];
}

export interface CreateSaleItemRequest {
  productoId: number;
  cantidad: number;
  precioUnitario: number;
  descuentoPorcentaje?: number;
  descuentoMonto?: number;
  notas?: string;
}

export interface CreatePaymentRequest {
  metodoPago: string;
  monto: number;
  referencia?: string;
  banco?: string;
  tipoTarjeta?: string;
  notas?: string;
}
