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

const providers = [
  AuthService,
  AuthGuard,
  RoleGuard,
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  importProvidersFrom(
    RouterModule.forRoot([
      { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
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
        path: 'dashboard', 
        loadComponent: () => import('./app/dashboard/dashboard.component').then(m => m.DashboardComponent), 
        canActivate: [AuthGuard] 
      },
      { 
        path: 'counter', 
        loadComponent: () => import('./app/counter/counter.component').then(m => m.CounterComponent),
        canActivate: [AuthGuard]
      },
      { 
        path: 'fetch-data', 
        loadComponent: () => import('./app/fetch-data/fetch-data.component').then(m => m.FetchDataComponent),
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
  .catch(err => console.log(err));