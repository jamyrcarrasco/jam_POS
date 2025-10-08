export interface User {
  id: number;
  username: string;
  role: string;
  firstName: string;
  lastName: string;
  email: string;
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
}
