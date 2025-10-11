import { BaseEntity } from '../../../core/models/base.model';

export interface Tax extends BaseEntity {
  nombre: string;
  descripcion?: string;
  tipo: string;
  porcentaje: number;
  montoFijo?: number;
  aplicaPorDefecto: boolean;
  activo: boolean;
}

