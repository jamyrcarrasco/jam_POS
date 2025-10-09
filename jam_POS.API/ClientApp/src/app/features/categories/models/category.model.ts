import { BaseEntity } from '../../../core/models/base.model';

export interface Category extends BaseEntity {
  nombre: string;
  descripcion?: string;
  color?: string;
  icono?: string;
  activo: boolean;
  productosCount?: number;
}

export interface CategoryFilter {
  searchTerm?: string;
  activo?: boolean;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  orderDescending?: boolean;
}

