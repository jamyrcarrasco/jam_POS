import { BaseEntity } from '../../../core/models/base.model';

export interface User extends BaseEntity {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  roleId: number;
  roleName: string;
  isActive: boolean;
  lastLoginAt?: Date;
}

export interface UserFilter {
  searchTerm?: string;
  roleId?: number;
  isActive?: boolean;
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  orderDescending?: boolean;
}

export interface CreateUserDto {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  roleId: number;
  isActive: boolean;
}

export interface UpdateUserDto {
  id: number;
  username: string;
  email: string;
  password?: string;
  firstName: string;
  lastName: string;
  roleId: number;
  isActive: boolean;
}

