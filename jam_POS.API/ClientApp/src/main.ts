import { enableProdMode, importProvidersFrom } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppComponent } from './app/app.component';
import { environment } from './environments/environment';
import { AuthGuard } from './app/guards/auth.guard';
import { AuthInterceptor } from './app/interceptors/auth.interceptor';


const providers = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  importProvidersFrom(
    RouterModule.forRoot([
      { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
      { path: 'login', loadComponent: () => import('./app/login/login.component').then(m => m.LoginComponent) },
      { path: 'dashboard', loadComponent: () => import('./app/dashboard/dashboard.component').then(m => m.DashboardComponent), canActivate: [AuthGuard] },
      { path: 'productos', loadComponent: () => import('./app/productos/productos.component').then(m => m.ProductosComponent), canActivate: [AuthGuard] },
      { path: 'counter', loadComponent: () => import('./app/counter/counter.component').then(m => m.CounterComponent) },
      { path: 'fetch-data', loadComponent: () => import('./app/fetch-data/fetch-data.component').then(m => m.FetchDataComponent) },
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
