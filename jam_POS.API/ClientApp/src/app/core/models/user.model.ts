export interface User {
  id: number;
  username: string;
  role: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface Empresa {
  id: number;
  nombre: string;
  nombreComercial: string;
  rnc?: string;
  direccion?: string;
  telefono?: string;
  email?: string;
  logoUrl?: string;
  pais?: string;
  ciudad?: string;
  codigoPostal?: string;
  plan: string;
  fechaVencimientoPlan?: Date;
  activo: boolean;
  createdAt: Date;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  role: string;
  firstName: string;
  lastName: string;
  expiresAt: string;
  empresa?: Empresa;
}
