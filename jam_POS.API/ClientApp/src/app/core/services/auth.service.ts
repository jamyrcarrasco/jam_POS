import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { User, LoginRequest, LoginResponse } from '../models/user.model';
import { API_ENDPOINTS } from '../constants/api-endpoints.constant';
import { APP_CONSTANTS } from '../constants/app-constants.constant';

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
    this.apiUrl = environment.apiUrl;
    this.loadUserFromStorage();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}${API_ENDPOINTS.AUTH.LOGIN}`, credentials)
      .pipe(
        tap(response => {
          this.storeAuthData(response);
          this.updateCurrentUser(response);
        })
      );
  }

  logout(): void {
    this.clearAuthData();
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(APP_CONSTANTS.STORAGE_KEYS.TOKEN);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
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
    return this.hasRole(APP_CONSTANTS.ROLES.SUPER_ADMIN);
  }

  isSeller(): boolean {
    return this.hasRole(APP_CONSTANTS.ROLES.SELLER);
  }

  isAdmin(): boolean {
    return this.hasRole(APP_CONSTANTS.ROLES.ADMIN);
  }

  isCashier(): boolean {
    return this.hasRole(APP_CONSTANTS.ROLES.CASHIER);
  }

  private storeAuthData(response: LoginResponse): void {
    localStorage.setItem(APP_CONSTANTS.STORAGE_KEYS.TOKEN, response.token);
    localStorage.setItem(APP_CONSTANTS.STORAGE_KEYS.USER, JSON.stringify({
      username: response.username,
      role: response.role,
      firstName: response.firstName,
      lastName: response.lastName
    }));
  }

  private clearAuthData(): void {
    localStorage.removeItem(APP_CONSTANTS.STORAGE_KEYS.TOKEN);
    localStorage.removeItem(APP_CONSTANTS.STORAGE_KEYS.USER);
  }

  private updateCurrentUser(response: LoginResponse): void {
    this.currentUserSubject.next({
      id: 0, // We don't have the ID in the response
      username: response.username,
      role: response.role,
      firstName: response.firstName,
      lastName: response.lastName,
      email: '' // We don't have the email in the response
    });
  }

  private loadUserFromStorage(): void {
    const userStr = localStorage.getItem(APP_CONSTANTS.STORAGE_KEYS.USER);
    const token = localStorage.getItem(APP_CONSTANTS.STORAGE_KEYS.TOKEN);
    
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
