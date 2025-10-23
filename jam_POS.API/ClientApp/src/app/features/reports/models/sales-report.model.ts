export interface SalesReport {
  totalVentas: number;
  totalImpuestos: number;
  totalDescuentos: number;
  cantidadVentas: number;
  totalItemsVendidos: number;
  promedioVenta: number;
  mes?: number;
  anio?: number;
  fechaInicio?: string;
  fechaFin?: string;
  periodoDescripcion: string;
  ventas: SalesReportSale[];
  productos: SalesReportProduct[];
}

export interface SalesReportSale {
  id: number;
  numeroVenta: string;
  fechaVenta: string;
  subtotal: number;
  totalImpuestos: number;
  totalDescuentos: number;
  total: number;
  estado: string;
  usuarioNombre: string;
  cantidadItems: number;
}

export interface SalesReportProduct {
  productoId: number;
  productoNombre: string;
  categoriaNombre?: string;
  cantidadVendida: number;
  totalVendido: number;
}

export interface SalesReportFilter {
  month?: number;
  year?: number;
  startDate?: string;
  endDate?: string;
  productId?: number;
  categoryId?: number;
}
