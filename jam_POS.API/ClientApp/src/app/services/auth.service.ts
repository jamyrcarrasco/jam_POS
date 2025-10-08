import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

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

export interface User {
  id: number;
  username: string;
  role: string;
  firstName: string;
  lastName: string;
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl: string;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.apiUrl = environment.apiUrl + 'auth';
    this.loadUserFromStorage();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => {
          // Store token and user info
          localStorage.setItem('token', response.token);
          localStorage.setItem('user', JSON.stringify({
            username: response.username,
            role: response.role,
            firstName: response.firstName,
            lastName: response.lastName
          }));
          
          // Update current user
          this.currentUserSubject.next({
            id: 0, // We don't have the ID in the response
            username: response.username,
            role: response.role,
            firstName: response.firstName,
            lastName: response.lastName,
            email: '' // We don't have the email in the response
          });
        })
      );
  }

  logout(): void {
    // Clear storage
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    
    // Clear current user
    this.currentUserSubject.next(null);
    
    // Navigate to login
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    // Check if token is expired
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Date.now() / 1000;
      return payload.exp > currentTime;
    } catch {
      return false;
    }
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  isSuperAdmin(): boolean {
    return this.hasRole('SuperAdmin');
  }

  isSeller(): boolean {
    return this.hasRole('Seller');
  }

  private loadUserFromStorage(): void {
    const userStr = localStorage.getItem('user');
    const token = localStorage.getItem('token');
    
    if (userStr && token && this.isAuthenticated()) {
      const userData = JSON.parse(userStr);
      this.currentUserSubject.next({
        id: 0,
        username: userData.username,
        role: userData.role,
        firstName: userData.firstName,
        lastName: userData.lastName,
        email: ''
      });
    }
  }
}
