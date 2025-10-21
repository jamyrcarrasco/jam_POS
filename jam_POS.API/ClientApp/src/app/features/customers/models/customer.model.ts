import { BaseEntity } from '../../../core/models/base.model';

export interface Customer extends BaseEntity {
  nombre: string;
  apellido?: string | null;
  nombreCompleto: string;
  email?: string | null;
  telefono?: string | null;
  documento?: string | null;
  direccion?: string | null;
  notas?: string | null;
  fechaNacimiento?: Date | null;
  activo: boolean;
}

export interface CustomerFilter {
  searchTerm?: string;
  activo?: boolean | null;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  orderDescending?: boolean;
}

export interface CreateCustomerDto {
  nombre: string;
  apellido?: string | null;
  email?: string | null;
  telefono?: string | null;
  documento?: string | null;
  direccion?: string | null;
  notas?: string | null;
  fechaNacimiento?: string | null;
  activo: boolean;
}

export interface UpdateCustomerDto extends CreateCustomerDto {
  id: number;
}
