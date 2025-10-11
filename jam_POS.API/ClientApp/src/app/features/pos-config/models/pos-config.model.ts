import { BaseEntity } from '../../../core/models/base.model';

export interface POSConfig extends BaseEntity {
  // Configuración de Recibos/Facturas
  prefijoRecibo: string;
  prefijoFactura: string;
  siguienteNumeroRecibo: number;
  siguienteNumeroFactura: number;
  mensajePieRecibo?: string;
  incluirLogoRecibo: boolean;

  // Comportamiento de Ventas
  impuestoPorDefectoId?: number;
  impuestoPorDefectoNombre?: string;
  permitirDescuentos: boolean;
  permitirDevoluciones: boolean;
  tiempoLimiteAnulacionMinutos: number;
  descuentoMaximoPorcentaje: number;

  // Configuración de Moneda
  monedaPorDefecto: string;
  simboloMoneda: string;
  decimalesMoneda: number;

  // Configuración de Impresión
  formatoPapel: string;
  imprimirAutomaticamente: boolean;
  imprimirCopiaCliente: boolean;

  // Configuración de Métodos de Pago
  efectivoHabilitado: boolean;
  tarjetaHabilitado: boolean;
  transferenciaHabilitado: boolean;
  creditoHabilitado: boolean;

  // Configuración de Modo Operación
  modoOfflineHabilitado: boolean;
  intervaloSincronizacionMinutos: number;
}
