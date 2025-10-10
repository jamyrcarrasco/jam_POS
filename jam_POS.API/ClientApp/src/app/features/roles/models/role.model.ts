import { BaseEntity } from '../../../core/models/base.model';

export interface Permission {
  id: number;
  name: string;
  module: string;
  description: string;
  isSystem: boolean;
  createdAt: Date;
}

export interface Role extends BaseEntity {
  name: string;
  description: string;
  isSystem: boolean;
  activo: boolean;
  usersCount: number;
  permissionsCount: number;
  permissions?: Permission[];
}

export interface PermissionsByModule {
  [module: string]: Permission[];
}

