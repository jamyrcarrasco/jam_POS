import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { API_ENDPOINTS } from '../constants/api-endpoints.constant';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();
    
    // If token exists and the request is not to the auth endpoints, add it to the header
    if (token && !this.isAuthEndpoint(request.url)) {
      const authRequest = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      return next.handle(authRequest);
    }
    
    return next.handle(request);
  }

  private isAuthEndpoint(url: string): boolean {
    return Object.values(API_ENDPOINTS.AUTH).some(endpoint => url.includes(endpoint));
  }
}
