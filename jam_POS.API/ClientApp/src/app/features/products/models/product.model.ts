import { BaseEntity } from '../../../core/models/base.model';

export interface Product extends BaseEntity {
  nombre: string;
  descripcion?: string;
  precio: number;
  stock: number;
  categoria?: string;
  codigoBarras?: string;
  imagenUrl?: string;
  precioCompra?: number;
  margenGanancia?: number;
  stockMinimo?: number;
  activo: boolean;
}

export interface ProductFilter {
  searchTerm?: string;
  categoria?: string;
  precioMin?: number;
  precioMax?: number;
  stockBajo?: boolean;
  activo?: boolean;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  orderDescending?: boolean;
}
