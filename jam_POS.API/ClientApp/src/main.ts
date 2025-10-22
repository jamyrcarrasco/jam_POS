import { enableProdMode, importProvidersFrom } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppComponent } from './app/app.component';
import { environment } from './environments/environment';
import { AuthGuard } from './app/core/guards/auth.guard';
import { RoleGuard } from './app/core/guards/role.guard';
import { AuthInterceptor } from './app/core/interceptors/auth.interceptor';
import { ErrorInterceptor } from './app/core/interceptors/error.interceptor';
import { AuthService } from './app/core/services/auth.service';
import { FileUploadService } from './app/core/services/file-upload.service';

const providers = [
  AuthService,
  FileUploadService,
  AuthGuard,
  RoleGuard,
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  importProvidersFrom(
    RouterModule.forRoot([
      { 
        path: '', 
        loadComponent: () => import('./app/features/landing/components/landing/landing.component').then(m => m.LandingComponent)
      },
      { 
        path: 'auth', 
        loadChildren: () => import('./app/features/auth/auth.module').then(m => m.AuthModule)
      },
      { 
        path: 'products', 
        loadChildren: () => import('./app/features/products/products.module').then(m => m.ProductsModule),
        canActivate: [AuthGuard]
      },
      { 
        path: 'configuraciones', 
        loadChildren: () => import('./app/features/settings/settings.module').then(m => m.SettingsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'ventas',
        loadChildren: () => import('./app/features/sales/sales.module').then(m => m.SalesModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'clientes',
        loadChildren: () => import('./app/features/customers/customers.module').then(m => m.CustomersModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./app/dashboard/dashboard.component').then(m => m.DashboardComponent),
        canActivate: [AuthGuard]
      },
      { path: '**', redirectTo: '/dashboard' }
    ]),
    HttpClientModule,
    BrowserAnimationsModule
  )
];

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, { providers })
  .catch(err => {
    // Bootstrap error handling
  });